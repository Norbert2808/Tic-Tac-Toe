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

        private readonly JsonHelper<Room> _jsonHelper;

        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        public RoomService()
        {
            _roomStorage = new List<Room>();
            _jsonHelper = new JsonHelper<Room>(Path);
        }

        public async Task<string> StartRoomAsync(string id, string login, RoomSettings settings)
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

        private async Task<string> CreateRoomAsync(string login, RoomSettings settings)
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
                return (true, new[] { room.LoginFirstPlayer, room.LoginSecondPlayer });

            if (!room.IsConnectionTimeOut())
                return (false, new[] { room.GetConnectionTime().ToString(@"dd\:mm\:ss") });

            DeleteRoom(room);
            throw new TimeoutException("Timeout");
        }

        public async Task<(bool, Move)> CheckMoveAsync(string id, string login)
        {
            var room = await FindRoomByIdAsync(id);

            if (room is null)
                throw new RoomException("Room not found.");

            var round = room.Rounds.Peek();
            var isFirstPlayer = room.LoginFirstPlayer.Equals(login, StringComparison.Ordinal);
            var rightMove = round.CheckOpponentsMove(isFirstPlayer);

            if (room.IsRoundTimeOut())
            {
                room.ConfirmFirstPlayer = false;
                room.ConfirmSecondPlayer = false;
                throw new TimeOutException("Time out.");
            }

            if (rightMove)
            {
                return round.CheckEndOfGame()
                    ? (true, round.LastMove!)
                    : (false, round.LastMove!);
            }

            return (false, null)!;
        }

        public async Task<bool> DoMoveAsync(string id, string login, Move move)
        {
            var room = await FindRoomByIdAsync(id);

            if (room is null)
                throw new RoomException("Room not found.");

            if (room.IsRoundTimeOut())
                throw new TimeOutException("Time out.");

            var round = room.Rounds.Peek();

            var isFirstPlayer = room.LoginFirstPlayer.Equals(login, StringComparison.Ordinal);

            var isValid = round.DoMove(move, isFirstPlayer);
            if (isValid)
            {
                room.LastMoveTime = DateTime.UtcNow;

                if (round.CheckEndOfGame())
                {
                    room.ConfirmFirstPlayer = false;
                    room.ConfirmSecondPlayer = false;
                    return true;
                }
                return false;
            }

            throw new RoomException("Not valid data.");
        }

        public async Task<(bool, string)> CheckConfirmationAsync(string id)
        {
            var room = await FindRoomByIdAsync(id);

            if (room is null)
                throw new RoomException("Room not found.");

            if (!room.IsCompleted)
                throw new AuthorizationException("Player left the room.");

            if (room.ConfirmFirstPlayer && room.ConfirmSecondPlayer)
            {
                _ = await _semaphoreSlim.WaitAsync(1);
                try
                {
                    room.Rounds.Push(new Round());
                    room.LastMoveTime = DateTime.UtcNow;
                }
                finally
                {
                    _ = _semaphoreSlim.Release();
                }

                return (true, null!);
            }

            return room.IsStartGameTimeOut()
                ? throw new TimeoutException("Time out")
                : (false, room.GetStartGameWaitingTime().ToString(@"dd\:mm\:ss"));
        }

        public async Task AppendConfirmationAsync(bool confirmation, string id)
        {
            var room = await FindRoomByIdAsync(id);

            if (room is null)
                throw new RoomException("Room not found.");

            if (room.ConfirmFirstPlayer == false)
            {
                room.ConfirmFirstPlayer = confirmation;
                room.ConfirmationTime = DateTime.UtcNow;
            }
            else
            {
                room.ConfirmSecondPlayer = confirmation;
            }
        }

        public async Task<bool> CheckPlayerPositionAsync(string id, string login)
        {
            var room = await FindRoomByIdAsync(id);

            if (room is null)
                throw new RoomException("Room not found.");

            if (!room.IsCompleted)
                throw new AuthorizationException();

            var isFirst = room.LoginFirstPlayer.Equals(login, StringComparison.Ordinal);

            return isFirst;
        }

        public async Task SurrenderAsync(string id, string login)
        {
            var room = await FindRoomByIdAsync(id);

            if (room is null)
                throw new RoomException("Room not found.");

            //ToDo Make surrender
        }

        public async Task<bool> ExitFromRoomAsync(string login, string id)
        {
            var room = await FindRoomByIdAsync(id);

            if (room is null)
                return false;

            if (room.LoginFirstPlayer.Equals(login, StringComparison.Ordinal))
            {
                room.ConfirmFirstPlayer = false;
                room.IsCompleted = false;
            }
            else
            {
                room.ConfirmSecondPlayer = false;
                room.IsCompleted = false;
            }

            if (room.ConfirmSecondPlayer == false
                && room.ConfirmFirstPlayer == false)
            {
                room.FinishRoomDate = DateTime.UtcNow;
                room.IsFinished = true;

                await _jsonHelper.AddObjectToFileAsync(room);
                DeleteRoom(room);
            }

            return true;
        }

        private Room? ConnectionToPublicRoom(string login)
        {
            foreach (var room in _roomStorage.Where(room => !room.IsCompleted && room.Settings.Type == RoomType.Public))
            {
                if (room.LoginFirstPlayer.Length == 0)
                {
                    room.LoginFirstPlayer = login;
                    if (room.LoginSecondPlayer.Length != 0)
                        room.IsCompleted = true;
                    return room;
                }

                room.LoginSecondPlayer = login;
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
            room.LoginSecondPlayer = login;
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
