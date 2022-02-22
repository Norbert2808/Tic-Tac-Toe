using Tic_Tac_Toe.Server.Models;

namespace Tic_Tac_Toe.Server.Tools
{
    public class UserBlocker : IBlocker
    {
        private readonly List<UserBlockInfo> _usersLoginBlockList;

        private readonly object _locker = new();

        public UserBlocker()
        {
            _usersLoginBlockList = new List<UserBlockInfo>();
        }

        public void UnBlock(string login)
        {
            lock (_locker)
            {
                _ = _usersLoginBlockList.RemoveAll(x =>
                      x.Login.Equals(login, StringComparison.Ordinal));
            }
        }

        public bool IsBlocked(string login)
        {
            lock (_locker)
            {
                var user = _usersLoginBlockList
                  .FirstOrDefault(x => x.Login.Equals(login, StringComparison.Ordinal));
                return user is not null && user.IsBlocked();
            }
        }

        public void ErrorTryLogin(string login)
        {
            lock (_locker)
            {
                var user = _usersLoginBlockList
                    .FirstOrDefault(x => x.Login.Equals(login, StringComparison.Ordinal));
                if (user is null)
                {
                    _usersLoginBlockList.Add(new UserBlockInfo(login));
                    _usersLoginBlockList.Last().TryEntry();
                }
                else
                {
                    user.TryEntry();
                }
            }
        }
    }
}
