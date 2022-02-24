using System.Net.Http.Formatting;
using TicTacToe.Client.DTO;
using TicTacToe.Client.Enums;

namespace TicTacToe.Client.Services.Impl;

public class GameService : IGameService
{
    private readonly HttpClient _httpClient;

    public string? RoomId { get; private set; }

    public GameService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> StartSessionAsync(RoomType roomType, string roomId, bool isConnect)
    {
        var settings = new SessionSettings(roomType, roomId, isConnect);
        
        var response = await _httpClient.PostAsync("api/Game/create_room", settings, new JsonMediaTypeFormatter());

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            RoomId = await response.Content.ReadAsStringAsync();
        }

        return response;
    }

    public async Task<HttpResponseMessage> CheckSecondPlayerAsync()
    {
        return await _httpClient.GetAsync("api/Game/check_room/" + RoomId);
    }
}
