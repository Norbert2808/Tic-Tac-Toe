namespace Tic_Tac_Toe.Client.Services;

public class StatisticService : IStatisticService
{
    private readonly HttpClient _httpClient;
    
    public StatisticService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
}
