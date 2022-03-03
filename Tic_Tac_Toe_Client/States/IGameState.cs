namespace TicTacToe.Client.States;

public interface IGameState : IGame, IState
{
    Task MakeMoveAsync();

    Task WaitMoveOpponentAsync();
}
