using TicTacToe.Server.Models;

namespace TicTacToe.Server.Services
{
    public interface IAccountService
    {
        Task UpdateAllUsersAccountAsync();

        bool FindAccountByLogin(string login);

        bool FindAccountByPassword(string password);

        List<UserAccount> GetStorage();

        Task AddAccountToStorageAsync(UserAccount account);
    }
}
