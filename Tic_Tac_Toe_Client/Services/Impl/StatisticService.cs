namespace Tic_Tac_Toe.Client.Services.Impl;

public class StatisticService : IStatisticService
{
    private readonly HttpClient _httpClient;
    
    public StatisticService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
}
