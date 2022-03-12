using TicTacToe.Client.Enums;

namespace TicTacToe.Client.Services
{
    public interface IStatisticService
    {
        Task<HttpResponseMessage> GetPrivateStatistic();

        Task<HttpResponseMessage> GetLeadersStatistic(SortingType type);
    }
}
