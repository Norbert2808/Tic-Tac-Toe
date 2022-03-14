using TicTacToe.Server.DTO;
using TicTacToe.Server.Enums;

namespace TicTacToe.Server.Services
{
    public interface IStatisticService
    {
        Task<PrivateStatisticDto> GetPrivateStatisticAsync(string login);

        Task<List<LeaderStatisticDto>> GetLeadersAsync(SortingType type);
    }
}
