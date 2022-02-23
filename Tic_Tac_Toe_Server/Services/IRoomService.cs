using Tic_Tac_Toe.Server.Models;

namespace Tic_Tac_Toe.Server.Services;

public interface IRoomService
{
    Task<string> CreateRoomAsync(string login, RoomSettings settings);

    Room? FindRoomAsync(string login);

    void AddRoomAsync(Room room);
    
    
}
