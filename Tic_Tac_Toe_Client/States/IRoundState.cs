namespace TicTacToe.Client.States
{
    public interface IRoundState : IExit, IState
    {
        Task<bool> WaitingStartGame();

        Task ShowEnemyBarAsync();
    }
}
