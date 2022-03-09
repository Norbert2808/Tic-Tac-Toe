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
                        _roomStorage.Add(room);
                    }

                    return room.RoomId;
                }

                if (settings.Type == RoomType.Private)
                {
                    if (settings.IsConnection)
                    {
                        var room = await ConnectionToPrivateRoomAsync(login, settings.RoomId);
                        return room is null ? null! : room.RoomId;
                    }
                    else
                    {
                        var room = new Room(login, settings);
                        _roomStorage.Add(room);
                        return room.RoomId;
                    }

                }

                if (settings.Type == RoomType.Practice)
                {
                    return null!;
                }
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }

            return null!;
        }

        public async Task<(bool, string[])> CheckRoomAsync(string id)
        {
            var room = await FindRoomByIdAsync(id);

            if (room is null)
                throw new RoomException("Room not found");

            if (room.IsCompleted)
                return (true, new[] { room.FirstPlayer.Login, room.SecondPlayer.Login });

            if (!room.TimeOuts.IsConnectionTimeOut())
                return (false, new[] { room.TimeOuts.GetConnectionTime().ToString(@"dd\:mm\:ss") });

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
                throw new RoomException("Room not found.");

            if (room.TimeOuts.IsRoundTimeOut())
                throw new TimeOutException("Time out.");


            var isFirstPlayer = room.FirstPlayer.Login.Equals(login, StringComparison.Ordinal);

            _roundService.DoMove(room, move, isFirstPlayer);
        }

        public async Task<(bool, string)> CheckConfirmationAsync(string id, string login)
        {
            var room = await FindRoomByIdAsync(id);

            if (room is null)
                throw new RoomException("Room not found.");

            if (!room.IsCompleted)
                throw new AccountException("Player left the room.");

            if (room.ConfirmFirstPlayer && room.ConfirmSecondPlayer)
            {
                await _semaphoreSlim.WaitAsync();
                try
                {
                    if (room.Rounds.Count == 0)
                    {
                        room.Rounds.Push(new Round());
                        room.TimeOuts.LastMoveTime = DateTime.UtcNow;
                    }
                    else
                    {
                        var round = room.Rounds.Peek();
                        if (round.IsFinished)
                        {
                            room.Rounds.Push(new Round());
                            room.TimeOuts.LastMoveTime = DateTime.UtcNow;
                        }
                    }
                }
                finally
                {
                    _ = _semaphoreSlim.Release();
                }

                return (true, null!);
            }

            return room.TimeOuts.IsStartGameTimeOut()
                ? throw new TimeoutException("Time out")
                : (false, room.TimeOuts.GetStartGameWaitingTime().ToString(@"dd\:mm\:ss"));
        }

        public async Task AppendConfirmationAsync(bool confirmation, string id)
        {
            var room = await FindRoomByIdAsync(id);

            if (room is null)
                throw new RoomException("Room not found.");

            if (room.ConfirmFirstPlayer == false)
            {
                room.ConfirmFirstPlayer = confirmation;
                room.TimeOuts.ConfirmationTime = DateTime.UtcNow;
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

            if (!room.IsCompleted)
                throw new AccountException();

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

        public async Task<bool> ExitFromRoomAsync(string login, string id)
        {
            var room = await FindRoomByIdAsync(id);

            if (room is null)
                return false;

            room.TimeOuts.FinishRoomDate = DateTime.UtcNow;

            if (room.Rounds.Count > 0)
                await _jsonHelper.AddObjectToFileAsync(room);

            DeleteRoom(room);
            return true;
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
