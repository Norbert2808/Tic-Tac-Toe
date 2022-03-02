using TicTacToe.Server.Models;

namespace TicTacToe.Server.Services;

public interface IRoomService
{
    Task<string> CreateRoomAsync(string login, RoomSettings settings);

    Room? ConnectionToPublicRoom(string login, string roomId);

    Task<Room?> ConnectionToPrivateRoomAsync(string login, string roomId);

    Task<Room?> FindRoomByIdAsync(string roomId);

    Task<Room?> AppendConfirmation(bool confirmation, string roomId, string login);

    Task<bool> ExitFromRoomAsync(string login, string id);

    void DeleteRoom(Room room);
}
