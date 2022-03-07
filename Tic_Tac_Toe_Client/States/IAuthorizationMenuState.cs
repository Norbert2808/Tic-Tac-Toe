namespace TicTacToe.Client.States
{
    public interface IAuthorizationMenuState : IState
    {
        Task ExecuteLoginAsync();

        Task ExecuteRegistrationAsync();
    }
}
