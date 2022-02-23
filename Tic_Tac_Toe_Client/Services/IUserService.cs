namespace TicTacToe.Client.Services
{
    internal interface IUserService
    {
        Task<HttpResponseMessage> LoginAsync(string login, string password);

        Task<HttpResponseMessage> RegistrationAsync(string login, string password);
    }
}
