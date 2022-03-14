using TicTacToe.Client.Enums;

namespace TicTacToe.Client.Services
{
    public interface IStatisticService
    {
        /// <summary>
        /// Sends the request to get private statistic.
        /// </summary>
        /// <returns>
        /// The task result contains <see cref="HttpResponseMessage"/>.
        /// </returns>
        Task<HttpResponseMessage> GetPrivateStatisticAsync();

        /// <summary>
        /// Sends the request to get leaders statistic.
        /// </summary>
        /// <param name="type">Sorting type <see cref="SortingType"/></param>
        /// <returns>
        /// The task result contains <see cref="HttpResponseMessage"/>
        /// </returns>
        Task<HttpResponseMessage> GetLeadersStatisticAsync(SortingType type);
    }
}
