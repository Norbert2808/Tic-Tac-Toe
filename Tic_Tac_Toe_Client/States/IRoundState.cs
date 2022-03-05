namespace TicTacToe.Client.States
{
    public interface IRoundState : IGame, IState
    {
        Task<bool> WaitingStartGame();

        Task<string> GetMessageFromResponseAsync(HttpResponseMessage response);

        Task ShowEnemyBarAsync();
    }
}
