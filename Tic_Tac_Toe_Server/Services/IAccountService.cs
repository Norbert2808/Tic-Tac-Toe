using TicTacToe.Server.Models;

namespace TicTacToe.Server.Services
{
    public interface IAccountService
    {
        Task UpdateAllUsersAccount();

        bool FindAccountByLogin(string login);

        bool FindAccountByPassword(string password);

        List<UserAccount> GetStorage();

        Task AddAccountToStorage(UserAccount account);
    }
}
