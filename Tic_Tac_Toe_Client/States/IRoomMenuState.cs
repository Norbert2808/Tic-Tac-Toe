using System.Globalization;
using TicTacToe.Client.DTO;
using TicTacToe.Client.Enums;

namespace TicTacToe.Client.States
{
    public interface IRoomMenuState : IState
    {
        /// <summary>
        /// Invoke method <see cref="Services.IGameService.StartRoomAsync(RoomSettingsDto)"/>,
        /// and processes response.
        /// <br>
        /// If response is <see cref="System.Net.HttpStatusCode.OK"/> will be invoked <see cref="WaitSecondPlayerAsync(string)"/>
        /// </br>
        /// </summary>
        /// <param name="type">Room type. <see cref="RoomType"/></param>
        /// <param name="roomId">Room id.</param>
        /// <param name="isConnecting">Indicates that the user wants to join
        /// an already existing room.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. <see cref="Task"/>
        /// </returns>
        Task StartConnectionWithRoomAsync(RoomType type, string roomId, bool isConnecting);

        /// <summary>
        /// Invoke <see cref="Services.IGameService.CheckRoomAsync"/> and processes response.
        /// If response status code is <see cref="System.Net.HttpStatusCode.OK"/> will be invoked
        /// <see cref="Impl.RoundMenuState.InvokeMenuAsync"/>.
        /// If status code is <see cref="System.Net.HttpStatusCode.Conflict"/> will be invoked
        /// <see cref="Services.IGameService.ExitFromRoomAsync"/>
        /// </summary>
        /// <param name="message">Message from response
        /// <see cref="Services.IGameService.StartRoomAsync(RoomSettingsDto)"/>.
        /// Invoke method <see cref="Services.IGameService.CheckRoomAsync"/>
        /// and processes the response.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. <see cref="Task"/>
        /// </returns>
        Task WaitSecondPlayerAsync(string message);

        /// <summary>
        /// Invoke main method <see cref="IState.InvokeMenuAsync"/> in the class
        /// <see cref="States.Impl.RoundMenuState"/>.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. <see cref="Task"/>
        /// </returns>
        Task InvokeRoundStateAsync();

        /// <summary>
        /// Invoke <see cref="Services.IGameService.ExitFromRoomAsync"/>.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. <see cref="Task"/>
        /// </returns>
        Task ExitFromRoomAsync();

    }
}
