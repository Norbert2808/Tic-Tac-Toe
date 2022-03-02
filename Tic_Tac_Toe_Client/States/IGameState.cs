namespace TicTacToe.Client.States;

public interface IGameState : IState
{
    Task WaitingStartGame();

    Task GameMenu();
    
    Task MakeMoveAsync();

    Task WaitMoveOpponentAsync();

    Task ExitAsync();

    Task EnemyBarMenu();
}
