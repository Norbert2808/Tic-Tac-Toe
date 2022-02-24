using TicTacToe.Server.Models;

namespace TicTacToe.Server.Services
{
    public interface IAccountService
    {
        Task UpdateAllUsersAccountAsync();

        Task<bool> FindAccountByLogin(string login);

        Task<bool> FindAccountByPassword(string password);

        List<UserAccount> GetStorage();

        Task AddAccountToStorageAsync(UserAccount account);
    }
}
