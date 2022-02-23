using TicTacToe.Server.Models;

namespace TicTacToe.Server.Services
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
