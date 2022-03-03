namespace TicTacToe.Client.States;

public interface IMainMenuState : IState
{
    Task ExecuteRoomMenuAsync();

    Task LogoutAsync();
    
    Task<string> GetMessageFromResponseAsync(HttpResponseMessage response);
}
