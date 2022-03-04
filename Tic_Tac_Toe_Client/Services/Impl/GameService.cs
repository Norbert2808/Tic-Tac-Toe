using System.Net;
using System.Net.Http.Formatting;
using TicTacToe.Client.DTO;
using TicTacToe.Client.Enums;

namespace TicTacToe.Client.Services.Impl
{
    public class GameService : IGameService
    {
        private readonly HttpClient _httpClient;

        public string? RoomId { get; private set; }

        public GameService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> StartRoomAsync(RoomType roomType, string roomId, bool isConnect)
        {
            var settings = new RoomSettingsDto(roomType, roomId, isConnect);

            var response = await _httpClient.PostAsync("api/Game/create_room",
                settings,
                new JsonMediaTypeFormatter());

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                RoomId = await response.Content.ReadAsStringAsync();
            }

            return response;
        }

        public async Task<HttpResponseMessage> CheckRoomAsync()
        {
            return await _httpClient.GetAsync("api/Game/check_room/" + RoomId);
        }

        public async Task<HttpResponseMessage> MakeMoveAsync(MoveDto move)
        {
            var response = await _httpClient.PostAsync("api/Game/move/" + RoomId,
                move,
                new JsonMediaTypeFormatter());
            return response;
        }

        public async Task<HttpResponseMessage> CheckMoveAsync()
        {
            return await _httpClient.GetAsync("api/Game/check_move/" + RoomId);
        }

        public async Task<HttpResponseMessage> SendConfirmationAsync()
        {
            return await _httpClient.PostAsync("api/Game/send_confirmation/" + RoomId,
                true,
                new JsonMediaTypeFormatter());
        }

        public async Task<HttpResponseMessage> CheckConfirmationAsync()
        {
            return await _httpClient.GetAsync("api/Game/check_confirmation/" + RoomId);
        }

        public async Task<bool> CheckPlayerPosition()
        {
            var response = await _httpClient.GetAsync("api/Game/check_position/" + RoomId);
            if (response.StatusCode == HttpStatusCode.OK)
            {
               return await response.Content.ReadAsAsync<bool>();  
            }
            return false;
        }

        public async Task<HttpResponseMessage> ExitFromRoomAsync()
        {
            return await _httpClient.GetAsync("api/Game/exit/" + RoomId);
        }
    }
}
