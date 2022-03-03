using TicTacToe.Server.Models;
using TicTacToe.Server.Tools;

namespace TicTacToe.Server.Services.Impl
{
    public sealed class AccountService : IAccountService
    {
        private const string Path = "usersStorage.json";

        private List<UserAccount> _accountsStorage;

        private readonly List<UserAccount> _activeAccounts;

        private readonly JsonHelper<UserAccount> _jsonHelper;

        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        private readonly object _locker = new();

        public AccountService()
        {
            _jsonHelper = new JsonHelper<UserAccount>(Path);
            _accountsStorage = new List<UserAccount>();
            _activeAccounts = new List<UserAccount>();
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
            await _semaphoreSlim.WaitAsync();
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
                _ = _semaphoreSlim.Release();
            }
        }

        public UserAccount FindAccountByLogin(string login)
        {
            return _accountsStorage.FirstOrDefault(x => x.Login.Equals(login, StringComparison.Ordinal))!;
        }

        public void RemoveActiveAccountByLogin(string login)
        {
            lock (_locker)
            {
                var account = FindAccountByLogin(login);
                _ = _activeAccounts.Remove(account);
            }
        }

        public void AddActiveAccount(UserAccount account)
        {
            lock (_locker)
            {
                _activeAccounts.Add(account);
            }
        }

        public bool IsActiveUserByLogin(string login)
        {
            var acc = _activeAccounts
                .FirstOrDefault(x => x.Login.Equals(login, StringComparison.Ordinal));
            return acc is not null;
        }
    }
}
