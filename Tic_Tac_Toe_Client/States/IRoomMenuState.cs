using TicTacToe.Client.Enums;

namespace TicTacToe.Client.States
{
    public interface IRoomMenuState : IState
    {
        /// <summary>
        /// Invoke method <see cref="Services.IGameService.StartRoomAsync(RoomType, string, bool)"/>,
        /// and processes response.
        /// </summary>
        /// <param name="type">Room type.<see cref="RoomType"/></param>
        /// <param name="roomId">Room id.</param>
        /// <param name="isConnecting">Indicates that the user wants to join
        /// an already existing room.</param>
        /// <returns>
        /// Returns <see cref="Task"/>
        /// </returns>
        Task StartConnectionWithRoomAsync(RoomType type, string roomId, bool isConnecting);


        /// <summary>
        /// Invoke <see cref="Services.IGameService.CheckRoomAsync"/> and processes response.
        /// <br>
        /// If <see cref="System.Net.HttpStatusCode.OK"/> will be invoked <see cref="Impl.RoundMenuState.InvokeMenuAsync"/>,
        /// if <see cref="System.Net.HttpStatusCode.Conflict"/> will be invoked <see cref="Services.IGameService.ExitFromRoomAsync"/>
        /// </br>
        /// </summary>
        /// <param name="message">Message from response 
        /// <see cref="Services.IGameService.StartRoomAsync(RoomType, string, bool)"/></param>.
        /// Invoke method <see cref="Services.IGameService.CheckRoomAsync"/>
        /// and processes the response.
        /// <returns>
        /// Returns <see cref="Task"/>
        /// </returns>
        Task WaitSecondPlayerAsync(string message);

    }
}
