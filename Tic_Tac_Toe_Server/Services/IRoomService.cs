using TicTacToe.Server.Models;

namespace TicTacToe.Server.Services
{
    public interface IRoomService
    {
        Task<string> StartRoomAsync(string id, string login, RoomSettingsDto settings);

        Task<(bool, string[])> CheckRoomAsync(string id);

        Task<(bool, MoveDto)> CheckMoveAsync(string id, string login);

        Task<bool> DoMoveAsync(string id, string login, MoveDto move);

        Task<(bool, string)> CheckConfirmationAsync(string id);

        Task AppendConfirmationAsync(bool confirmation, string id);

        Task<bool> CheckPlayerPositionAsync(string id, string login);

        Task SurrenderAsync(string id, string login);

        Task<bool> ExitFromRoomAsync(string login, string id);
    }
}
