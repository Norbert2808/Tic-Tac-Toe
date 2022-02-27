namespace TicTacToe.Client.States;

public interface IMainMenuState : IState
{
    Task ExecuteGameMenuAsync();

    Task LogoutAsync();
    
    Task<string> GetMessageFromResponseAsync(HttpResponseMessage response);
}
