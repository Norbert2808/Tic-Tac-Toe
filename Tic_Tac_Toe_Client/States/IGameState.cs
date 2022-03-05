namespace TicTacToe.Client.States
{
    public interface IGameState : IGame, IState
    {
        Task<bool> MakeMoveAsync();

        Task WaitMoveOpponentAsync();
    }
}
