using TicTacToe.Server.DTO;
using TicTacToe.Server.Models;

namespace TicTacToe.Server.Services
{
    public interface IRoomService
    {
        Task<string> StartRoomAsync(string id, string login, RoomSettingsDto settings);

        Task<(bool, string)> CheckRoomAsync(string id);

        Task<RoundStateDto?> CheckMoveAsync(string id, string login);

        Task DoMoveAsync(string id, string login, MoveDto move);

        Task<(bool, string)> CheckConfirmationAsync(string id, string login);

        Task AppendConfirmationAsync(bool confirmation, string id);

        Task<ResultsDto> GetResultAsync(string id);

        Task<RoundStateDto> CheckStateRoundAsync(string id, string login);

        Task SurrenderAsync(string id, string login);

        Task ExitFromRoomAsync(string id);
    }
}
