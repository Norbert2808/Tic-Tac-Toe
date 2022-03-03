using TicTacToe.Server.Enums;
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

        public async Task<string> CreateRoomAsync(string login, RoomSettings settings)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                if (settings.Type == RoomType.Public)
                {
                    var room = ConnectionToPublicRoom(login, settings.RoomId);

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

        public Room? ConnectionToPublicRoom(string login, string roomId)
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

        public async Task<Room?> ConnectionToPrivateRoomAsync(string login, string roomId)
        {
            var room = await FindRoomByIdAsync(roomId);
            if (room is null)
                return null;
            room.LoginSecondPlayer = login;
            room.IsCompleted = true;
            return room;
        }

        public async Task<Room?> FindRoomByIdAsync(string roomId)
        {
            return await Task.FromResult(_roomStorage
                .FirstOrDefault(x => x.RoomId.Equals(roomId, StringComparison.Ordinal)));
        }

        public async Task<Room?> AppendConfirmation(bool confirmation, string roomId, string login)
        {
            var room = await FindRoomByIdAsync(roomId);
            if (room is null)
                return null;
            if (room.ConfirmFirstPlayer == false)
            {
                room.ConfirmFirstPlayer = true;
                room.ConfirmationTime = DateTime.UtcNow;
            }
            else
            {
                room.ConfirmSecondPlayer = true;
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

        public void DeleteRoom(Room room)
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
