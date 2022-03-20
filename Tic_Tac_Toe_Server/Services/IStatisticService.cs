using TicTacToe.Server.DTO;
using TicTacToe.Server.Enums;

namespace TicTacToe.Server.Services
{
    public interface IStatisticService
    {
        /// <summary>
        /// Sends the response with private statistic at predefined time intervals.
        /// </summary>
        /// <param name="login">User login.</param>
        /// <param name="startDate">Time to start looking.</param>
        /// <param name="endDate">Time to end looking</param>
        /// <returns>
        /// The task result contains <see cref="PrivateStatisticDto"/>
        /// </returns>
        Task<PrivateStatisticDto> GetPrivateStatisticAsync(string login,
            DateTime startDate, DateTime endDate);

        /// <summary>
        /// Sends the response with leader statistic.
        /// </summary>
        /// <param name="type">Sorting type. <see cref="SortingType"/></param>
        /// <returns>
        /// The task result contains <see cref="List{T}"/>
        /// <list type="bullet">
        /// <item><see cref="LeaderStatisticDto"/></item>
        /// </list>
        /// </returns>
        Task<List<LeaderStatisticDto>> GetLeadersAsync(SortingType type);
    }
}
