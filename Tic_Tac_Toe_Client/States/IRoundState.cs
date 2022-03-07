namespace TicTacToe.Client.States
{
    public interface IRoundState : IGame, IState
    {
        Task<bool> WaitingStartGame();

        Task ShowEnemyBarAsync();
    }
}
