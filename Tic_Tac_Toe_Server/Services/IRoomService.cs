using TicTacToe.Server.DTO;

namespace TicTacToe.Server.Services
{
    public interface IRoomService
    {
        /// <summary>
        /// Find room by id and registers user there. If room not found,
        /// user create new room and sends the response.
        /// </summary>
        /// <param name="id">Room id.</param>
        /// <param name="login">User login.</param>
        /// <param name="settings">Room settings.</param>
        /// <returns>
        /// The task result contains <see cref="string"/>.
        /// </returns>
        /// <exception cref="Exceptions.RoomException"> Throws if room already taken or
        /// token for private room does not exist.</exception>
        Task<string> StartRoomAsync(string id, string login, RoomSettingsDto settings);

        /// <summary>
        /// Checks the room for a second player. If the player has not logged in,
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
        /// Invoke method <see cref="IRoundService.CheckMove"/>
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
        /// Next step invoke <see cref="IRoundService.DoMove"/>
        /// </summary>
        /// <param name="id">Room id.</param>
        /// <param name="login">User login.</param>
        /// <param name="move">Player movement <see cref="MoveDto"/></param>
        /// <returns>
        /// A task that represents the asynchronous operation. <see cref="Task"/>
        /// </returns>
        /// <exception cref="Exceptions.RoomException"> - Throws if room not found or
        /// when opponent left from room.</exception>
        /// <exception cref="Exceptions.TimeOutException"> - Throws if move time expired.</exception>
        Task DoMoveAsync(string id, string login, MoveDto move);

        /// <summary>
        /// Checks if the second player has accepted the game. If two players
        /// confirmed the game, creates new round.
        /// </summary>
        /// <param name="id">Room id.</param>
        /// <param name="login">User login.</param>
        /// <returns>
        /// The task result contains <see cref="Tuple"/>.
        /// <list type="bullet">
        /// <item><see cref="bool"/> - Confirm game or no.</item>
        /// <item><see cref="string"/> - Message.</item>
        /// </list>
        /// </returns>
        /// <exception cref="Exceptions.RoomException"> - Throws if room not found or opponent
        /// left from room.</exception>
        /// <exception cref="Exceptions.TimeOutException"> - Throws if confirmation time expired.</exception>
        Task<(bool, string)> CheckConfirmationAsync(string id, string login);

        /// <summary>
        /// Confirms the player’s readiness for the round.
        /// </summary>
        /// <param name="confirmation">Player's confirmation.</param>
        /// <param name="id">Room id.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. <see cref="Task"/>
        /// </returns>
        /// <exception cref="Exceptions.RoomException"> - Throws if player didn't confirm the game or
        /// opponent left from room.</exception>
        /// <exception cref="Exceptions.TimeOutException"> - Throws if </exception>
        Task AppendConfirmationAsync(bool confirmation, string id);

        /// <summary>
        /// Sends the response about game result.
        /// </summary>
        /// <param name="id">Room id.</param>
        /// <returns>
        /// The task result contains <see cref="ResultsDto"/>
        /// </returns>
        /// <exception cref="Exceptions.RoomException"> - Throws if room not found.</exception>
        Task<ResultsDto> GetResultAsync(string id);

        /// <summary>
        /// Sends the response about round state.
        /// </summary>
        /// <param name="id">Room id.</param>
        /// <param name="login">User login.</param>
        /// <returns>
        /// The task result contains <see cref="RoundStateDto"/>
        /// </returns>
        /// <exception cref="Exceptions.RoomException"> - Throws if room not found.</exception>
        Task<RoundStateDto> CheckStateRoundAsync(string id, string login);

        /// <summary>
        /// Invoke <see cref="IRoundService.ExitFormRound"/>.
        /// </summary>
        /// <param name="id">Room id.</param>
        /// <param name="login">User login.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. <see cref="Task"/>
        /// </returns>
        /// <exception cref="Exceptions.RoomException">- Throws if room not found.</exception>
        Task SurrenderAsync(string id, string login);

        /// <summary>
        /// Exit from room and save information about room state.
        /// </summary>
        /// <param name="id">Room id.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. <see cref="Task"/>
        /// </returns>
        /// <exception cref="Exceptions.RoomException"> - Throws if room completed.</exception>
        Task ExitFromRoomAsync(string id);
    }
}
