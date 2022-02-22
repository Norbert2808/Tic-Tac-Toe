using Tic_Tac_Toe.Server.Models;

namespace Tic_Tac_Toe.Server.Services
{
    public interface IAccountService
    {
        Task FindAllUsersAccount();

        //Task<UserAccount?> FindAccountByLogin(string login);

        Task<bool> FindAccountByLogin(string login);
        
        Task<bool> FindAccountByPassword(string password);

        List<UserAccount> GetStorage();

        Task AddAccountToStorage(UserAccount account);
    }
}
