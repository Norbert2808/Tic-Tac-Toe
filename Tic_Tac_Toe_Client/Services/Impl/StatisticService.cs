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

        public async Task<HttpResponseMessage> GetPrivateStatisticDto()
        {
            return await _httpClient.GetAsync(ControllerPath + "private");
        }
    }
}
