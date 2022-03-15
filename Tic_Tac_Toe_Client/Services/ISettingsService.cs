
using TicTacToe.Client.DTO;
using TicTacToe.Client.Enums;
using TicTacToe.Client.Models;

namespace TicTacToe.Client.Services
{
    public interface ISettingsService
    {
        Task SerializeAsync(TimeOut timeOut);

        Task<RoomSettingsDto> ConfigureSettingsAsync(string login, RoomType type, string roomId,
           bool isConnecting);
    }
}
