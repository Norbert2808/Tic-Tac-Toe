using TicTacToe.Server.Models;

namespace TicTacToe.Server.Services
{
    public interface IAccountService
    {
        Task InvokeLoginAsync(UserAccount account);

        Task InvokeRegistrationAsync(UserAccount account);

        void RemoveActiveAccountByLogin(string login);

    }
}
