using TicTacToe.Server.Exceptions;
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

        private readonly IBlocker _blocker;

        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        public AccountService(IBlocker blocker)
        {
            _blocker = blocker;
            _jsonHelper = new JsonHelper<UserAccount>(Path);
            _accountsStorage = new List<UserAccount>();
            _activeAccounts = new List<UserAccount>();
        }

        public async Task InvokeLoginAsync(UserAccount account)
        {
            await UpdateAllUsersAccountAsync();

            var loginIsExist = IsAccountExistByLoginAsync(account.Login);

            if (!await loginIsExist)
                throw new AuthorizationException("Input login does not exist");

            if (_blocker.IsBlocked(account.Login))
            {
                throw new TimeoutException("You’re blocked for 1 minute. You try log-in three times.");
            }

            var passwordIsExist = await IsAccountExistByPasswordAsync(account.Password);

            if (!passwordIsExist)
            {
                _blocker.ErrorTryLogin(account.Login);
                throw new AuthorizationException("Password is wrong!");
            }

            _blocker.UnBlock(account.Login);

            if (IsActiveUserByLogin(account.Login))
                throw new AuthorizationException("User have already logged-in");

            AddActiveAccount(account);
        }

        public async Task InvokeRegistrationAsync(UserAccount account)
        {
            await UpdateAllUsersAccountAsync();

            if (await IsAccountExistByLoginAsync(account.Login))
            {
                throw new AuthorizationException("User with such login already registered");
            }

            AddActiveAccount(account);

            await AddAccountToStorageAsync(account);
        }

        private async Task UpdateAllUsersAccountAsync()
        {
            _accountsStorage = await _jsonHelper.DeserializeAsync();
        }

        public List<UserAccount> GetStorage()
        {
            return _accountsStorage;
        }

        private async Task<bool> IsAccountExistByLoginAsync(string login)
        {
            return await Task.FromResult(_accountsStorage
                .Any(x => x.Login.Equals(login, StringComparison.OrdinalIgnoreCase)));
        }

        private async Task<bool> IsAccountExistByPasswordAsync(string password)
        {
            return await Task.FromResult(_accountsStorage
                .Any(x => x.Password.Equals(password, StringComparison.Ordinal)));
        }


        private async Task AddAccountToStorageAsync(UserAccount account)
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

        private UserAccount FindAccountByLogin(string login)
        {
            return _accountsStorage.FirstOrDefault(x => x.Login.Equals(login, StringComparison.Ordinal))!;
        }

        public void RemoveActiveAccountByLogin(string login)
        {
            _semaphoreSlim.Wait();
            try
            {
                var account = FindAccountByLogin(login);
                _ = _activeAccounts.Remove(account);
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        private void AddActiveAccount(UserAccount account)
        {
            _semaphoreSlim.Wait();
            try
            {
                _activeAccounts.Add(account);
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        private bool IsActiveUserByLogin(string login)
        {
            var acc = _activeAccounts
                .FirstOrDefault(x => x.Login.Equals(login, StringComparison.Ordinal));
            return acc is not null;
        }
    }
}
