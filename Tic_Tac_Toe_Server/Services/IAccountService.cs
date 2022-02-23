using Tic_Tac_Toe.Server.Models;

namespace Tic_Tac_Toe.Server.Services
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
