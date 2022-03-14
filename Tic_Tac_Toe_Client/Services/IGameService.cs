using TicTacToe.Client.DTO;
using TicTacToe.Client.Enums;

namespace TicTacToe.Client.Services
{
    public interface IGameService
    {
        /// <summary>
        /// Sends the request to create a room or log into an existing one.
        /// </summary>
        /// <param name="roomType">Room type. <see cref="RoomType"/></param>
        /// <param name="roomId">Room id.</param>
        /// <param name="isConnect">Indicates that the user wants to join
        /// an already existing room.</param>
        /// <returns>
        /// The task result contains <see cref="HttpResponseMessage"/>.
        /// </returns>
        Task<HttpResponseMessage> StartRoomAsync(RoomType roomType, string roomId, bool isConnect);

        /// <summary>
        /// Sends the request for checks if the second player has logged in room. 
        /// or not called time out./
        /// </summary>
        /// <returns>
        /// The task result contains <see cref="HttpResponseMessage"/>.
        /// </returns>
        Task<HttpResponseMessage> CheckRoomAsync();

        /// <summary>
        /// Sends the request for player movement.
        /// </summary>
        /// <param name="move">Player movement data.</param>
        /// <returns>
        /// The task result contains <see cref="HttpResponseMessage"/>.
        /// </returns>
        Task<HttpResponseMessage> MakeMoveAsync(MoveDto move);

        /// <summary>
        /// Sends the request for check if the second player made a move or time out.
        /// </summary>
        /// <returns>
        /// The task result contains <see cref="HttpResponseMessage"/>.
        /// </returns>
        Task<HttpResponseMessage> CheckMoveAsync();

        /// <summary>
        /// Sends the data that player confirm new round.
        /// </summary>
        /// <returns>
        /// The task result contains <see cref="HttpResponseMessage"/>.
        /// </returns>
        Task<HttpResponseMessage> SendConfirmationAsync();

        /// <summary>
        /// Sends the request for check if the second player made a confirm or time out.
        /// </summary>
        /// <returns>
        /// The task result contains <see cref="HttpResponseMessage"/>.
        /// </returns>
        Task<HttpResponseMessage> CheckConfirmationAsync();

        /// <summary>
        /// Asks the server for game results.
        /// </summary>
        /// <returns>
        /// The task result contains <see cref="HttpResponseMessage"/>.
        /// </returns>
        Task<HttpResponseMessage> GetResultsAsync();

        /// <summary>
        /// Gets round state from server.
        /// </summary>
        /// <returns>
        /// The task result contains <see cref="HttpResponseMessage"/>.
        /// </returns>
        Task<HttpResponseMessage> CheckRoundStateAsync();

        /// <summary>
        /// Sends the request that the player wants to surrender.
        /// </summary>
        /// <returns>
        /// The task result contains <see cref="HttpResponseMessage"/>.
        /// </returns>
        Task<HttpResponseMessage> SurrenderAsync();

        /// <summary>
        /// Sends the request that the player exit from room.
        /// </summary>
        /// <returns>
        /// The task result contains <see cref="HttpResponseMessage"/>.
        /// </returns>
        Task<HttpResponseMessage> ExitFromRoomAsync();
    }
}
