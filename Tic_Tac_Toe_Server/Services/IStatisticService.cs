using TicTacToe.Server.Models;

namespace TicTacToe.Server.Services
{
    public interface IStatisticService
    {
        Task<PrivateStatisticDto> GetPrivateStatistic(string login);
    }
}
