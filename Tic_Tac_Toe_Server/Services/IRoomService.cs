using TicTacToe.Server.Enum;
using TicTacToe.Server.Models;

namespace TicTacToe.Server.Services;

public interface IRoomService
{
    Task<string> CreateRoomAsync(string login, RoomSettings settings);

    Room? ConnectionToPublicRoom(string login, string roomId);

    Task<Room?> ConnectionToPrivateRoom(string login, string roomId);

    Task<Room?> FindRoomByIdAsync(string roomId);
}
