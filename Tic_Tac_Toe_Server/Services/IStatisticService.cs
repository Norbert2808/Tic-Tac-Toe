using TicTacToe.Server.Models;

namespace TicTacToe.Server.Services
{
    public interface IStatisticService
    {
        Task<PrivateStatistic> GetPrivateStatistic(string login);
    }
}
