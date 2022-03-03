namespace TicTacToe.Client.States;

public interface IRoundState : IGame, IState
{
    Task WaitingStartGame();

    Task<string> GetMessageFromResponseAsync(HttpResponseMessage response);
}
