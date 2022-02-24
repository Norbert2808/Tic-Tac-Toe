using Microsoft.AspNetCore.Mvc;
using TicTacToe.Server.Enum;
using TicTacToe.Server.Models;
using TicTacToe.Server.Tools;

namespace TicTacToe.Server.Services.Impl;

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
            var room = FindFreeRoom(login);

            if (room is null)
            {
                room = new Room(login, settings);
                _roomStorage.Add(room);
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

    public Room? FindFreeRoom(string login)
    {
        foreach (var room in _roomStorage)
        {
            if (room.IsClosed)
                continue;

            room.LoginSecondPlayer = login;
            room.IsClosed = true;
            return room;
        }

        return null;
    }

    public async Task<Room?> FindRoomByIdAsync(string roomId)
    {
        return await Task.FromResult(_roomStorage
            .FirstOrDefault(x => x.RoomId.Equals(roomId, StringComparison.Ordinal)));
    }
    
}
