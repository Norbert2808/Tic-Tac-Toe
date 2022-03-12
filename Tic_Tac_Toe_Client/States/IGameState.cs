namespace TicTacToe.Client.States
{
    public interface IGameState : IState
    {
        /// <summary>
        /// Execute movement menu and
        /// invoke method <see cref="Services.IGameService.MakeMoveAsync(DTO.MoveDto)",
        /// and processes the response.
        /// </summary>
        /// <returns>
        /// Returns <see cref="bool"/>
        /// </returns>
        Task<bool> MakeMoveAsync();

        /// <summary>
        /// Execute method <see cref="Services.IGameService.CheckMoveAsync"/>,
        /// and processes the response.
        /// </summary>
        /// <returns>
        /// Returns <see cref="Task"/>
        /// </returns>
        Task WaitMoveOpponentAsync();

        /// <summary>
        /// Execute method <see cref="Services.IGameService.SurrenderAsync"/>,
        /// and processes the response.
        /// </summary>
        /// <returns>
        /// Returns <see cref="Task"/>
        /// </returns>
        Task SurrenderAsync();
    }
}
