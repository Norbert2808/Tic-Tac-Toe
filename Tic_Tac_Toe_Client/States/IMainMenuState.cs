namespace TicTacToe.Client.States
{
    public interface IMainMenuState : IState
    {
        Task ExecuteRoomMenuAsync();

        Task LogoutAsync();
    }
}
