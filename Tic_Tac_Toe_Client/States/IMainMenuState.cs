namespace TicTacToe.Client.States
{
    public interface IMainMenuState : IState
    {
        /// <summary>
        /// Invoke main room menu. <see cref="Impl.RoomMenuState.InvokeMenuAsync"/>
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. <see cref="Task"/>
        /// </returns>
        Task ExecuteRoomMenuAsync();

        /// <summary>
        /// Invoke method <see cref="Services.IUserService.LogoutAsync"/>
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. <see cref="Task"/>
        /// </returns>
        Task LogoutAsync();
    }
}
