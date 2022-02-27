namespace TicTacToe.Client.States;

public interface IGameState : IState
{
    Task MakeMoveAsync();

    Task WaitMoveOpponentAsync();
}
