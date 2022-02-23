namespace TicTacToe.Client.States;

public interface IMainMenuState : IState
{
    Task ExecuteGameMenu();
}
