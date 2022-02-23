using TicTacToe.Server.Models;
using TicTacToe.Server.Tools;

namespace TicTacToe.Server.Services.Impl
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

        public async Task UpdateAllUsersAccount()
        {
            _accountsStorage = await _jsonHelper.DeserializeAsync();
        }

        public List<UserAccount> GetStorage()
        {
            return _accountsStorage;
        }

        public bool FindAccountByLogin(string login)
        {
            return _accountsStorage.Any(x => x.Login.Equals(login, StringComparison.OrdinalIgnoreCase));
        }

        public bool FindAccountByPassword(string password)
        {
            return _accountsStorage.Any(x => x.Password.Equals(password, StringComparison.Ordinal));
        }


        public async Task AddAccountToStorage(UserAccount account)
        {
            _accountsStorage.Add(account);
            if (_accountsStorage.Count == 1)
                await _jsonHelper.SerializeAsync(_accountsStorage);
            else
                await _jsonHelper.AddAccountToFile(account);
        }

    }
}
