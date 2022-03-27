using TicTacToe.Server.DTO;
using TicTacToe.Server.Exceptions;
using TicTacToe.Server.Tools;

namespace TicTacToe.Server.Services.Impl
{
    public class AccountService : IAccountService
    {
        private List<UserAccountDto> _accountsStorage;

        private readonly List<UserAccountDto> _activeAccounts;

        private readonly IJsonHelper<UserAccountDto> _jsonHelper;

        private readonly IBlocker _blocker;

        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        public AccountService(IBlocker blocker, IJsonHelper<UserAccountDto> jsonHelper)
        {
            _blocker = blocker;
            _jsonHelper = jsonHelper;
            _accountsStorage = new List<UserAccountDto>();
            _activeAccounts = new List<UserAccountDto>();
        }

        public async Task InvokeLoginAsync(UserAccountDto account)
        {
            await UpdateAllUsersAccountAsync();

            var loginIsExist = await IsAccountExistByLoginAsync(account.Login);

            if (!loginIsExist)
                throw new AccountException("Input login does not exist");

            if (_blocker.IsBlocked(account.Login))
                throw new TimeoutException("You blocked for 1 minute. You try log-in three times.");

            var passwordIsExist = await IsAccountExistByPasswordAsync(account.Password);

            if (!passwordIsExist)
            {
                _blocker.ErrorTryLogin(account.Login);
                throw new AccountException("Wrong password!");
            }

            _blocker.UnBlock(account.Login);

            if (IsActiveUserByLogin(account.Login))
                throw new AccountException("User have already logged-in");

            AddActiveAccount(account);
        }

        public async Task InvokeRegistrationAsync(UserAccountDto account)
        {
            await UpdateAllUsersAccountAsync();

            if (await IsAccountExistByLoginAsync(account.Login))
                throw new AccountException("User with such login already registered");

            AddActiveAccount(account);

            await AddAccountToStorageAsync(account);
        }

        private async Task UpdateAllUsersAccountAsync()
        {
            _accountsStorage = await _jsonHelper.DeserializeAsync();
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

        private async Task AddAccountToStorageAsync(UserAccountDto account)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                _accountsStorage.Add(account);
                await _jsonHelper.AddObjectToFileAsync(account);
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }

        private UserAccountDto FindAccountByLogin(string login)
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

        private void AddActiveAccount(UserAccountDto account)
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
