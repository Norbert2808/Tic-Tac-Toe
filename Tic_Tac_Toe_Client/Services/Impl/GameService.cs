using System.Net.Http.Formatting;
using TicTacToe.Client.DTO;
using TicTacToe.Client.Enums;

namespace TicTacToe.Client.Services.Impl;

public class GameService : IGameService
{
    private readonly HttpClient _httpClient;
    
    public GameService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> StartSessionAsync(RoomType roomType, string roomId)
    {
        var settings = new SessionSettings(roomType, roomId);
        
        var response = await _httpClient.PostAsync("api/Game/create_room", settings, new JsonMediaTypeFormatter());

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            _httpClient.DefaultRequestHeaders.Clear();
        }

        return response;
    }
}
