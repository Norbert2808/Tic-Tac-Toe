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
            var room = ConnectionToPublicRoom(login, settings.RoomId);
    
            if (room is null)
            {
                room = new Room(login, settings);
                _roomStorage.Add(room);
            }

            return room.RoomId;   
        }

        if (settings.Type == RoomType.Private && settings.IsConnection)
        {
            var room = await ConnectionToPrivateRoom(login, settings.RoomId);
            return room is null ? null : room.RoomId;
        }
        else
        {
            var room = new Room(login, settings);
            _roomStorage.Add(room);
            return room.RoomId;
        }
    }

    public Room? ConnectionToPublicRoom(string login, string roomId)
    {
        foreach (var room in _roomStorage)
        {
            if (room.IsCompleted)
                continue;
            
            if (room.LoginFirstPlayer is null || room.LoginFirstPlayer.Length == 0)
            { 
                room.LoginFirstPlayer = login; 
                return room;
            } 
            room.LoginSecondPlayer = login; 
            room.IsCompleted = true; 
            return room;
        }

        return null;
    }

    public async Task<Room?> ConnectionToPrivateRoom(string login, string roomId)
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
}
