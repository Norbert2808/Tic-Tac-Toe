using Tic_Tac_Toe.Server.Models;

namespace Tic_Tac_Toe.Server.Service
{
    public interface IAccountService
    {
        Task FindAllUsersAccount();

        Task<UserAccount?> FindAccountByLogin(string login);

        List<UserAccount> GetStorage();

    }
}
