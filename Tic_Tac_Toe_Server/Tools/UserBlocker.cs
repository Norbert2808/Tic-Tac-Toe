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
                _usersLoginBlockList.RemoveAll(x => 
                    x.Login.Equals(login, StringComparison.Ordinal));
            }
        }

        public bool IsBlocked(string login)
        {
            lock (_locker)
            {
              var user = _usersLoginBlockList
                  .Select(x => x)
                    .Where(x => x.Login.Equals(login, StringComparison.Ordinal))
                    .FirstOrDefault();
              return user is null ? false : user.IsBlocked();
            }
        }

        public void ErrorTryLogin(string login)
        {
            lock (_locker)
            {
                var user = _usersLoginBlockList
                    .Select(x => x)
                    .Where(x => x.Login.Equals(login, StringComparison.Ordinal))
                    .FirstOrDefault();
                if(user is null)
                    _usersLoginBlockList.Add(new UserBlockInfo(login));
                else
                    user.TryEntry();
            }
        }
    }
}
