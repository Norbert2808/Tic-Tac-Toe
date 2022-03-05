using TicTacToe.Server.Enums;
using TicTacToe.Server.Exceptions;
using TicTacToe.Server.Models;
using TicTacToe.Server.Tools;

namespace TicTacToe.Server.Services.Impl
{
    public class RoomService : IRoomService
    {
        private const string Path = "sessionsInfo.json";

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

            var round = room.Rounds.Peek();

            var isFirstPlayer = room.LoginFirstPlayer.Equals(login, StringComparison.Ordinal);

            var isValid = round.DoMove(move, isFirstPlayer);
            if (isValid)
            {
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
            {
                throw new AuthorizationException("Player left the room.");
            }

            if (room.ConfirmFirstPlayer && room.ConfirmSecondPlayer)
            {
                room.Rounds.Push(new Round());
                return (true, null!);
            }

            return room.IsStartGameTimeOut()
                ? throw new TimeoutException("Time out")
                : (false, room.GetStartGameWaitingTime().ToString(@"dd\:mm\:ss"));
        }

        public async Task AppendConfirmationAsync(bool confirmation, string id)
        {
            var room = await AddConfirmationAsync(confirmation, id);

            if (room is null)
                throw new RoomException("Room not found.");
            if (room.IsStartGameTimeOut())
                throw new TimeoutException("Time out");
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

        private async Task<Room?> AddConfirmationAsync(bool confirmation, string roomId)
        {
            var room = await FindRoomByIdAsync(roomId);
            if (room is null)
                return null;
            if (room.ConfirmFirstPlayer == false)
            {
                room.ConfirmFirstPlayer = confirmation;
                room.ConfirmationTime = DateTime.UtcNow;
            }
            else
            {
                room.ConfirmSecondPlayer = confirmation;
            }

            return room;
        }

        public async Task<bool> ExitFromRoomAsync(string login, string id)
        {
            var room = await FindRoomByIdAsync(id);

            if (room is null)
                return false;

            if (room.LoginFirstPlayer.Equals(login, StringComparison.Ordinal))
            {
                room.LoginFirstPlayer = "";
                room.IsCompleted = false;
            }
            else
            {
                room.LoginSecondPlayer = "";
                room.IsCompleted = false;
            }

            if (room.LoginFirstPlayer.Length == 0
                && room.LoginSecondPlayer.Length == 0)
            {
                DeleteRoom(room);
            }

            return true;
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
