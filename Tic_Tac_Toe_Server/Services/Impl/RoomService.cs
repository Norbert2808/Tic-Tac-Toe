using Tic_Tac_Toe.Server.Enum;
using Tic_Tac_Toe.Server.Models;
using Tic_Tac_Toe.Server.Tools;

namespace Tic_Tac_Toe.Server.Services.Impl;

public class RoomService : IRoomService
{
    private const string Path = "sessionsInfo.json";
    
    private readonly List<Room> _roomStorage;

    private JsonHelper<Room> _jsonHelper;

    public RoomService()
    {
        _roomStorage = new List<Room>();
        _jsonHelper =  new JsonHelper<Room>(Path);
    }
    
    public async Task<string> CreateRoomAsync(string login, RoomSettings settings)
    {
        if (settings.Type == RoomType.Public)
        {
            var room = FindRoomAsync(login);

            if (room is null)
            {
                room = new Room(login, settings);
                AddRoomAsync(room);
            }

            return room.RoomId;
        }

        if (settings.Type == RoomType.Private)
        {
            
        }

        if (settings.Type == RoomType.Practice)
        {
            
        }
        return "";
    }

    public Room? FindRoomAsync(string login)
    {
        foreach (var room in _roomStorage)
        {
            if (room.IsClosed)
                continue;
            
            if (room.LoginFirstPlayer == login)
            {
                _roomStorage.Remove(room);
                continue;
            }

            room.LoginSecondPlayer = login;
            return room;
        }

        return null;
    }

    public void AddRoomAsync(Room room)
    {
        _roomStorage.Add(room);
    }
}
