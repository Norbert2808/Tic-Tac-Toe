using Tic_Tac_Toe.Server.Models;
using Tic_Tac_Toe.Server.Tools;

namespace Tic_Tac_Toe.Server.Service
{
    public sealed class AccountService : IAccountService
    {
        private const string Path = "usersStorage.json";

        private List<UserAccount> _accountsStorage;

        private readonly JsonHelper<UserAccount> _jsonHelper;

        public AccountService()
        {
            _jsonHelper = new JsonHelper<UserAccount>(Path);
            _accountsStorage = new List<UserAccount>();
        }

        public async Task FindAllUsersAccount()
        {
            _accountsStorage = await _jsonHelper.DeserializeAsync();
        }

        public List<UserAccount> GetStorage()
        {
            return _accountsStorage;
        }

        public async Task<UserAccount?> FindAccountByLogin(string login)
        {
            await FindAllUsersAccount();

            return _accountsStorage.FirstOrDefault(x => x.Login.Equals(login, StringComparison.OrdinalIgnoreCase));
        }

        public async Task AddAccountToStorage(UserAccount account)
        {
            _accountsStorage.Add(account);
            await _jsonHelper.SerializeAsync(_accountsStorage);
        }

        public async Task<bool> CheckForExistLogin(string login)
        {
            await FindAllUsersAccount();

            return _accountsStorage.Any(a => a.Login.Equals(login, StringComparison.OrdinalIgnoreCase));
        }
    }
}
