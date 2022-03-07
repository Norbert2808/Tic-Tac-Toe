namespace TicTacToe.Client.States
{
    public interface ILeaderMenuState : IState
    {
        Task<string> GetMessageFromResponseAsync(HttpResponseMessage response);
    }
}
