using TicTacToe.Server.DTO;
using TicTacToe.Server.Enums;
using TicTacToe.Server.Exceptions;
using TicTacToe.Server.Models;
using TicTacToe.Server.Tools;

namespace TicTacToe.Server.Services.Impl
{
    public class RoomService : IRoomService
    {
        private const string Path = "roomInfo.json";

        private readonly List<Room> _roomStorage;

        private readonly IRoundService _roundService;

        private readonly JsonHelper<Room> _jsonHelper;

        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        public RoomService(IRoundService roundService)
        {
            _roomStorage = new List<Room>();
            _roundService = roundService;
            _jsonHelper = new JsonHelper<Room>(Path);
        }

        public async Task<string> StartRoomAsync(string id, string login, RoomSettingsDto settings)
        {
            var room = await FindRoomByIdAsync(id);

            if (room is not null
                && room.Settings.Type == RoomType.Private
                && room.IsCompleted)
            {
                throw new RoomException("Room's already taken!");
            }

            var response = await CreateRoomAsync(login, settings);
            return response is null
                ? throw new RoomException("Such a token does not exist.")
                : response;
        }

        private async Task<string> CreateRoomAsync(string login, RoomSettingsDto settings)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                if (settings.Type == RoomType.Public)
                {
                    var room = ConnectionToPublicRoom(login);

                    if (room is null)
                    {
                        room = new Room(login, settings);
                        room.Times.ActionTimeInRoom = DateTime.UtcNow;
                        _roomStorage.Add(room);
                    }
                    room.Times.ActionTimeInRoom = DateTime.UtcNow;
                    return room.RoomId;
                }

                if (settings.Type == RoomType.Private)
                {
                    if (settings.IsConnection)
                    {
                        var room = await ConnectionToPrivateRoomAsync(login, settings.RoomId);
                        room.Times.ActionTimeInRoom = DateTime.UtcNow;
                        return room is null ? null! : room.RoomId;
                    }
                    else
                    {
                        var room = new Room(login, settings);
                        room.Times.ActionTimeInRoom = DateTime.UtcNow;
                        _roomStorage.Add(room);
                        return room.RoomId;
                    }

                }

                if (settings.Type == RoomType.Practice)
                {
                    var room = ConnectionToPracticeRoom(login, settings);
                    room.Times.ActionTimeInRoom = DateTime.UtcNow;
                    _roomStorage.Add(room);
                    return room.RoomId;
                }
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }

            return null!;
        }

        public async Task<(bool, string)> CheckRoomAsync(string id)
        {
            var room = await FindRoomByIdAsync(id);

            if (room is null)
                throw new RoomException("Second player leaves the room and room was deleting.");

            if (room.IsCompleted)
                return (true, string.Empty);

            if (!room.Times.IsConnectionTimeOut())
                return (false, room.Times.GetConnectionTime().ToString(@"dd\:mm\:ss"));

            DeleteRoom(room);
            throw new TimeoutException("Timeout");
        }

        public async Task<RoundStateDto?> CheckMoveAsync(string id, string login)
        {
            var room = await FindRoomByIdAsync(id);

            if (room is null)
                throw new RoomException("Room not found.");

            var isFirst = room.FirstPlayer.Login.Equals(login, StringComparison.Ordinal);

            return _roundService.CheckMove(room, isFirst);
        }

        public async Task DoMoveAsync(string id, string login, MoveDto move)
        {
            var room = await FindRoomByIdAsync(id);

            if (room is null)
                throw new RoomException("Second player leaves the room and room was deleting.");

            if (room.Times.IsRoundTimeOut())
            {
                if (room.IsBot)
                {
                    room.SecondPlayer.Wins++;
                    room.Rounds.Peek().IsFinished = true;
                }
                room.Times.ActionTimeInRoom = DateTime.UtcNow;

                throw new TimeOutException("Time out.");
            }


            var isFirstPlayer = room.FirstPlayer.Login.Equals(login, StringComparison.Ordinal);

            _roundService.DoMove(room, move, isFirstPlayer);
        }

        public async Task<(bool, string)> CheckConfirmationAsync(string id, string login)
        {
            var room = await FindRoomByIdAsync(id);

            if (room is null)
                throw new RoomException("Player left the room.");

            if (room.ConfirmFirstPlayer && room.ConfirmSecondPlayer)
            {
                await _semaphoreSlim.WaitAsync();
                try
                {
                    if (room.Rounds.Count == 0)
                    {
                        _roundService.CreateNewRound(room);
                    }
                    else
                    {
                        var round = room.Rounds.Peek();
                        if (round.IsFinished)
                        {
                            _roundService.CreateNewRound(room);
                        }
                    }
                }
                finally
                {
                    _ = _semaphoreSlim.Release();
                }

                return (true, null!);
            }

            return room.Times.IsStartGameTimeOut()
                ? throw new TimeoutException("Your opponent didn't confirm the game. Room was closed.")
                : (false, room.Times.GetStartGameWaitingTime().ToString(@"dd\:mm\:ss"));
        }

        public async Task AppendConfirmationAsync(bool confirmation, string id)
        {
            var room = await FindRoomByIdAsync(id);

            if (room is null)
            {
                throw new RoomException("You didn't confirm the game or your opponent left the room." +
                        "Room was closed.");
            }

            if (room.Times.IsRoomActionTimeOut())
                throw new TimeOutException("Time out. You were inactive inside the room for two minutes");

            room.Times.ActionTimeInRoom = DateTime.UtcNow;

            if (room.IsBot)
                room.ConfirmSecondPlayer = confirmation;

            if (room.ConfirmFirstPlayer == false)
            {
                room.ConfirmFirstPlayer = confirmation;
                room.Times.ConfirmationTime = DateTime.UtcNow;
            }
            else
            {
                room.ConfirmSecondPlayer = confirmation;
            }
        }

        public async Task<RoundStateDto> CheckStateRoundAsync(string id, string login)
        {
            var room = await FindRoomByIdAsync(id);

            if (room is null)
                throw new RoomException("Room not found.");

            var isFirst = room.FirstPlayer.Login.Equals(login, StringComparison.Ordinal);

            return _roundService.CheckState(room, isFirst);

        }

        public async Task SurrenderAsync(string id, string login)
        {
            var room = await FindRoomByIdAsync(id);

            if (room is null)
                throw new RoomException("Room not found.");

            var isFirst = room.FirstPlayer.Login.Equals(login, StringComparison.Ordinal);

            _roundService.ExitFormRound(room, isFirst);
        }

        public async Task ExitFromRoomAsync(string id)
        {
            var room = await FindRoomByIdAsync(id);

            if (room is null)
                throw new RoomException("Room was completed.");

            room.Times.FinishRoomDate = DateTime.UtcNow;

            if (room.Rounds.Count > 0)
                await _jsonHelper.AddObjectToFileAsync(room);

            DeleteRoom(room);
        }

        public async Task<ResultsDto> GetResultAsync(string id)
        {
            var room = await FindRoomByIdAsync(id);

            return room is null
                ? throw new RoomException("Room not found.")
                : new ResultsDto
                {
                    LoginFirstPlayer = room.FirstPlayer.Login,
                    LoginSecondPlayer = room.SecondPlayer.Login,
                    WinFirst = room.FirstPlayer.Wins,
                    WinSecond = room.SecondPlayer.Wins
                };
        }

        private Room? ConnectionToPublicRoom(string login)
        {
            foreach (var room in _roomStorage.Where(room => !room.IsCompleted && room.Settings.Type == RoomType.Public))
            {
                if (room.FirstPlayer.Login.Length == 0)
                {
                    room.FirstPlayer.Login = login;
                    if (room.SecondPlayer.Login.Length != 0)
                        room.IsCompleted = true;
                    return room;
                }

                room.SecondPlayer.Login = login;
                room.IsCompleted = true;
                return room;
            }
            return null;
        }

        private async Task<Room?> ConnectionToPrivateRoomAsync(string login, string roomId)
        {
            var room = await FindRoomByIdAsync(roomId);
            if (room is null)
                return null;

            room.SecondPlayer.Login = login;
            room.IsCompleted = true;
            return room;
        }

        private Room ConnectionToPracticeRoom(string login, RoomSettingsDto settings)
        {
            var room = new Room(login, settings)
            {
                SecondPlayer = new Player("Bot", 0),
                ConfirmSecondPlayer = true,
                IsCompleted = true
            };

            return room;
        }

        private async Task<Room?> FindRoomByIdAsync(string roomId)
        {
            return await Task.FromResult(_roomStorage
                .FirstOrDefault(x => x.RoomId.Equals(roomId, StringComparison.Ordinal)));
        }

        private void DeleteRoom(Room room)
        {
            _semaphoreSlim.Wait();
            try
            {
                _ = _roomStorage.Remove(room);
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }
    }
}
