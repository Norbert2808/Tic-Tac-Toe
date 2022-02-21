using System.Collections.Concurrent;
using Tic_Tac_Toe.Server.Models;

namespace Tic_Tac_Toe.Server.Service
{
    public interface IAccountService
    {
        Task FindAllUsersAccount();

        List<UserAccount> GetStorage();

    }
}
