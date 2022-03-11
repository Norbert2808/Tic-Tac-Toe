namespace TicTacToe.Client.States
{
    public interface IGameState : IExit, IState
    {
        Task<bool> MakeMoveAsync();

        Task WaitMoveOpponentAsync();
    }
}
