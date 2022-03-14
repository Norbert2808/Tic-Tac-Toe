using TicTacToe.Client.Enums;

namespace TicTacToe.Client.Services.Impl
{
    public class StatisticService : IStatisticService
    {
        private readonly HttpClient _httpClient;

        private const string ControllerPath = "api/statistic/";

        public StatisticService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> GetPrivateStatisticAsync()
        {
            return await _httpClient.GetAsync(ControllerPath + "private");
        }

        public async Task<HttpResponseMessage> GetLeadersStatisticAsync(SortingType type)
        {
            return await _httpClient.GetAsync(ControllerPath + $"leaders/{type}");
        }
    }
}
