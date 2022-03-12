namespace TicTacToe.Client.States
{
    public interface IRoundMenuState : IState
    {
        /// <summary>
        /// Sends the confirmation <see cref="Services.IGameService.SendConfirmationAsync"/>
        /// and invoke method <see cref="WaitConfirmationSecondPlayer"/>
        /// </summary>
        /// <returns>
        /// Returns <see cref="bool"/>
        /// </returns>
        Task<bool> WaitingStartGame();

        /// <summary>
        /// Invoke method <see cref="Services.IGameService.GetResultsAsync"/>
        /// and processes the response also draw opponents and play count to display.
        /// </summary>
        /// <returns>
        /// Returns <see cref="Task"/>
        /// </returns>
        Task ShowEnemyBarAsync();

        /// <summary>
        /// Invoke method <see cref="Services.IGameService.CheckConfirmationAsync"/>
        /// and processes the response.
        /// </summary>
        /// <returns>
        /// Returns <see cref="bool"/>
        /// </returns>
        Task<bool> WaitConfirmationSecondPlayer();

        /// <summary>
        /// Invoke method <see cref="Services.IGameService.ExitFromRoomAsync"/>
        /// </summary>
        /// <returns>
        /// Return <see cref="Task"/>
        /// </returns>
        Task ExitAsync();
    }
}
