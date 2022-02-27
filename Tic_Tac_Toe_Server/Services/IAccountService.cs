using TicTacToe.Server.Models;

namespace TicTacToe.Server.Services
{
    public interface IAccountService
    {
        Task UpdateAllUsersAccountAsync();

        Task<bool> IsAccountExistByLoginAsync(string login);

        Task<bool> IsAccountExistByPasswordAsync(string password);

        List<UserAccount> GetStorage();

        Task AddAccountToStorageAsync(UserAccount account);

        bool IsActiveUserByLogin(string login);

        Task<UserAccount> FindAccountByLoginAsync(string login);

        Task RemoveActiveAccountByLoginAsync(string login);

        void AddActiveAccount(UserAccount account);
    }
}
