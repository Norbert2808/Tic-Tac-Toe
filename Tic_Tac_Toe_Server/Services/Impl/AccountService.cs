using TicTacToe.Server.Models;
using TicTacToe.Server.Tools;

namespace TicTacToe.Server.Services.Impl
{
    public sealed class AccountService : IAccountService
    {
        private const string Path = "usersStorage.json";

        private List<UserAccount> _accountsStorage;

        private readonly JsonHelper<UserAccount> _jsonHelper;

        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        public AccountService()
        {
            _jsonHelper = new JsonHelper<UserAccount>(Path);
            _accountsStorage = new List<UserAccount>();
        }

        public async Task UpdateAllUsersAccountAsync()
        {
            _accountsStorage = await _jsonHelper.DeserializeAsync();
        }

        public List<UserAccount> GetStorage()
        {
            return _accountsStorage;
        }

        public async Task<bool> FindAccountByLogin(string login)
        {
            return await Task.FromResult(_accountsStorage
                .Any(x => x.Login.Equals(login, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<bool> FindAccountByPassword(string password)
        {
            return await Task.FromResult(_accountsStorage
                .Any(x => x.Password.Equals(password, StringComparison.Ordinal)));
        }


        public async Task AddAccountToStorageAsync(UserAccount account)
        {
            await  _semaphoreSlim.WaitAsync();
            try
            {
                _accountsStorage.Add(account);
                if (_accountsStorage.Count == 1)
                    await _jsonHelper.SerializeAsync(_accountsStorage);
                else
                    await _jsonHelper.AddAccountToFile(account);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

    }
}
