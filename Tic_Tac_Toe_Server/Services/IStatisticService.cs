using TicTacToe.Server.DTO;
using TicTacToe.Server.Enums;

namespace TicTacToe.Server.Services
{
    public interface IStatisticService
    {
        Task<PrivateStatisticDto> GetPrivateStatisticAsync(string login,
            DateTime startDate, DateTime endDate);

        Task<List<LeaderStatisticDto>> GetLeadersAsync(SortingType type);
    }
}
