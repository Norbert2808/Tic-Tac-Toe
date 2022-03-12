using TicTacToe.Server.DTO;
using TicTacToe.Server.Enums;

namespace TicTacToe.Server.Services
{
    public interface IStatisticService
    {
        Task<PrivateStatisticDto> GetPrivateStatistic(string login);

        Task<List<LeaderStatisticDto>> GetLeaders(SortingType type);
    }
}
