using TicTacToe.Server.Models;

namespace TicTacToe.Server.Services;

public interface IRoomService
{
    Task<string> CreateRoomAsync(string login, RoomSettings settings);

    Room? FindFreeRoom(string login);

    Task<Room?> FindRoomByIdAsync(string roomId);
}
