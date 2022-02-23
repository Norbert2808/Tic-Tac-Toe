namespace Tic_Tac_Toe.Client.Services;

public class GameService : IGameService
{
    private readonly HttpClient _httpClient;
    
    public GameService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
}
