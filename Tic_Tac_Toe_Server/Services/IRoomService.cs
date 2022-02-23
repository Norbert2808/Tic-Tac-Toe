using TicTacToe.Server.Models;

namespace TicTacToe.Server.Services;

public interface IRoomService
{
    Task<string> CreateRoomAsync(string login, RoomSettings settings);

    Room? FindRoomAsync(string login);

    void AddRoomAsync(Room room);
    
    
}
