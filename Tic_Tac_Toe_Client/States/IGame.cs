namespace TicTacToe.Client.States;

public interface IGame
{
    Task ExitFromRoomAsync();
    
    Task ShowEnemyBar();
}
