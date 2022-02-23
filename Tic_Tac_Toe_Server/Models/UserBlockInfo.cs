namespace TicTacToe.Server.Models;

public class UserBlockInfo
{
    public string Login { get; }
    
    public int CounterEntry { get; private set; }

    public DateTime TimeTryEntry { get; private set; }

    private readonly TimeSpan _blockTime = TimeSpan.FromSeconds(60);

    public bool IsBlocked()
    {
        return CounterEntry >= 3 && DateTime.UtcNow - TimeTryEntry < _blockTime;
    }

    public void TryEntry()
    {
        CounterEntry++;
        TimeTryEntry = DateTime.UtcNow;
    }

    public UserBlockInfo(string login)
    {
        Login = login;
        CounterEntry = 1;
    }
}
