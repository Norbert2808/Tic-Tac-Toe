using System.Net.Http.Formatting;

namespace TicTacToe.Client.Services.Impl
{
    public class StatisticService : IStatisticService
    {
        private readonly HttpClient _httpClient;

        public StatisticService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> GetPrivateStatisticDto()
        {
            return await _httpClient.PostAsync("api/Statistic/private", Login, new JsonMediaTypeFormatter());
        }
    }
}
