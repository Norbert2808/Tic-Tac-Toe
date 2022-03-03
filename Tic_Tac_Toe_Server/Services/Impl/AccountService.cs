using System.Collections.Concurrent;
using TicTacToe.Server.Models;
using TicTacToe.Server.Tools;

namespace TicTacToe.Server.Services.Impl
{
    public sealed class AccountService : IAccountService
    {
        private const string Path = "usersStorage.json";

        private List<UserAccount> _accountsStorage;

        private readonly ConcurrentBag<UserAccount> _activeAccounts;

        private readonly JsonHelper<UserAccount> _jsonHelper;

        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        public AccountService()
        {
            _jsonHelper = new JsonHelper<UserAccount>(Path);
            _accountsStorage = new List<UserAccount>();
            _activeAccounts = new ConcurrentBag<UserAccount>();
        }

        public async Task UpdateAllUsersAccountAsync()
        {
            _accountsStorage = await _jsonHelper.DeserializeAsync();
        }

        public List<UserAccount> GetStorage()
        {
            return _accountsStorage;
        }

        public async Task<bool> IsAccountExistByLoginAsync(string login)
        {
            return await Task.FromResult(_accountsStorage
                .Any(x => x.Login.Equals(login, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<bool> IsAccountExistByPasswordAsync(string password)
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
        
        public Task<UserAccount> FindAccountByLoginAsync(string login)
        {
            return Task.FromResult(_accountsStorage.FirstOrDefault(x => x.Login.Equals(login, StringComparison.Ordinal))!);
        }

        public async Task RemoveActiveAccountByLoginAsync(string login)
        {
            var account = await FindAccountByLoginAsync(login);
               _activeAccounts.TryTake(out account);
        }

        public void AddActiveAccount(UserAccount account)
        {
            _activeAccounts.Add(account);
        }

        public bool IsActiveUserByLogin(string login)
        {
            var acc = _activeAccounts
                .FirstOrDefault(x => x.Login.Equals(login, StringComparison.Ordinal)); 
            return acc is not null;
        }
    }
}
