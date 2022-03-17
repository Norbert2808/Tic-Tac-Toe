using TicTacToe.Server.DTO;

namespace TicTacToe.Server.Services
{
    public interface IRoomService
    {
        /// <summary>
        /// Find room by id and registers user there. If room not found,
        /// user create new room and sends the response.
        /// </summary>
        /// <param name="id">Room id</param>
        /// <param name="login">User login</param>
        /// <param name="settings">Room settings</param>
        /// <returns>
        /// The task result contains <see cref="String"/>.
        /// </returns>
        /// <exception cref="Exceptions.RoomException"> Throws if room already taken or
        /// token for private room does not exist.</exception>
        Task<string> StartRoomAsync(string id, string login, RoomSettingsDto settings);

        /// <summary>
        /// Checks the room for a second player.If the player has not logged in,
        /// the timeout is transmitted and sends the response.
        /// </summary>
        /// <param name="id">Room id.</param>
        /// <returns>
        /// The task result contains <see cref="Tuple"/>.
        /// <list type="bullet">
        /// <item><see cref="bool"/> - Completed room or no.</item>
        /// <item><see cref="string"/> - For connection time.</item>
        /// </list>
        /// </returns>
        /// <exception cref="Exceptions.RoomException"> Throws if room not found or
        /// seconds player left from room.</exception>
        /// <exception cref="Exceptions.TimeOutException"> Throws if connection time expires.</exception>
        Task<(bool, string)> CheckRoomAsync(string id);

        /// <summary>
        /// Checks if the second player made a move.
        /// Invoke method <see cref="Services.IRoundService.CheckMove"/>
        /// </summary>
        /// <param name="id">Room id.</param>
        /// <param name="login">User login.</param>
        /// <returns>
        /// The task result contains <see cref="RoundStateDto"/>
        /// </returns>
        /// <exception cref="Exceptions.RoomException">Throws if room not found or
        /// second player left from room</exception>
        /// <exception cref="Exceptions.TimeOutException">Throws if round time expires</exception>
        /// <exception cref="Exceptions.GameException">Throws if opponent surrendered.</exception>
        Task<RoundStateDto?> CheckMoveAsync(string id, string login);

        /// <summary>
        /// Checks for time out and makes a move.
        /// Next step invoke <see cref="Services.IRoundService.DoMove"/>
        /// </summary>
        /// <param name="id">Room id.</param>
        /// <param name="login">User login.</param>
        /// <param name="move">Player movement <see cref="MoveDto"/></param>
        /// <returns>
        /// A task that represents the asynchronous operation. <see cref="Task"/>
        /// </returns>
        Task DoMoveAsync(string id, string login, MoveDto move);

        Task<(bool, string)> CheckConfirmationAsync(string id, string login);

        Task AppendConfirmationAsync(bool confirmation, string id);

        Task<ResultsDto> GetResultAsync(string id);

        Task<RoundStateDto> CheckStateRoundAsync(string id, string login);

        Task SurrenderAsync(string id, string login);

        Task ExitFromRoomAsync(string id);
    }
}
