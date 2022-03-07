using TicTacToe.Server.Models;

namespace TicTacToe.Server.Services
{
    public interface IAccountService
    {
        Task InvokeLoginAsync(UserAccountDto account);

        Task InvokeRegistrationAsync(UserAccountDto account);

        void RemoveActiveAccountByLogin(string login);

    }
}
