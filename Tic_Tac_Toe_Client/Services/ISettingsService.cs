
using TicTacToe.Client.DTO;
using TicTacToe.Client.Enums;
using TicTacToe.Client.Models;

namespace TicTacToe.Client.Services
{
    public interface ISettingsService
    {
        /// <summary>
        /// Serialize data in json file.
        /// </summary>
        /// <param name="timeOut"></param>
        /// <returns>
        /// A task that represents the asynchronous operation. <see cref="Task"/>
        /// </returns>
        Task SerializeAsync(TimeOut timeOut);

        /// <summary>
        /// Configure room settings.
        /// </summary>
        /// <param name="login">User login</param>
        /// <param name="type"> Room type</param>
        /// <param name="roomId"> Room id</param>
        /// <param name="isConnecting">Flag that indicates that the player
        /// wants to connect to a private room</param>
        /// <returns>
        /// The task result contains <see cref="RoomSettingsDto"/>.
        /// </returns>
        Task<RoomSettingsDto> ConfigureSettingsAsync(string login, RoomType type, string roomId,
           bool isConnecting);
    }
}
