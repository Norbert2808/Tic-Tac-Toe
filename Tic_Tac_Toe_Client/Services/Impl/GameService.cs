using System.Net;
using System.Net.Http.Formatting;
using TicTacToe.Client.DTO;

namespace TicTacToe.Client.Services.Impl
{
    public class GameService : IGameService
    {
        private readonly HttpClient _httpClient;

        private const string ControllerPath = "api/game/";

        public string? RoomId { get; private set; }

        public GameService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> StartRoomAsync(RoomSettingsDto settingsDto)
        {
            var response = await _httpClient.PostAsync(ControllerPath + "create-room",
                settingsDto,
                new JsonMediaTypeFormatter());

            if (response.StatusCode == HttpStatusCode.OK)
            {
                RoomId = await response.Content.ReadAsStringAsync();
            }

            return response;
        }

        public async Task<HttpResponseMessage> CheckRoomAsync()
        {
            return await _httpClient.GetAsync(ControllerPath + $"check-room/{RoomId}");
        }

        public async Task<HttpResponseMessage> MakeMoveAsync(MoveDto move)
        {
            var response = await _httpClient.PostAsync(ControllerPath + $"move/{RoomId}",
                move,
                new JsonMediaTypeFormatter());
            return response;
        }

        public async Task<HttpResponseMessage> CheckMoveAsync()
        {
            return await _httpClient.GetAsync(ControllerPath + $"check-move/{RoomId}");
        }

        public async Task<HttpResponseMessage> SendConfirmationAsync()
        {
            return await _httpClient.PostAsync(ControllerPath + $"send-confirmation/{RoomId}",
                true,
                new JsonMediaTypeFormatter());
        }

        public async Task<HttpResponseMessage> CheckConfirmationAsync()
        {
            return await _httpClient.GetAsync(ControllerPath + $"check-confirmation/{RoomId}");
        }

        public async Task<HttpResponseMessage> CheckRoundStateAsync()
        {
            return await _httpClient.GetAsync(ControllerPath + $"check-round-state/{RoomId}");
        }

        public async Task<HttpResponseMessage> SurrenderAsync()
        {
            return await _httpClient.GetAsync(ControllerPath + $"surrender/{RoomId}");
        }

        public async Task<HttpResponseMessage> ExitFromRoomAsync()
        {
            return await _httpClient.DeleteAsync(ControllerPath + $"exit/{RoomId}");
        }

        public async Task<HttpResponseMessage> GetResultsAsync()
        {
            return await _httpClient.GetAsync(ControllerPath + $"get-results/{RoomId}");
        }
    }
}
