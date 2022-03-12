using System.Net;
using System.Net.Http.Formatting;
using TicTacToe.Client.DTO;
using TicTacToe.Client.Enums;

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

        public async Task<HttpResponseMessage> StartRoomAsync(RoomType roomType, string roomId, bool isConnect)
        {
            var settings = new RoomSettingsDto(roomType, roomId, isConnect);

            var response = await _httpClient.PostAsync(ControllerPath + "create_room",
                settings,
                new JsonMediaTypeFormatter());

            if (response.StatusCode == HttpStatusCode.OK)
            {
                RoomId = await response.Content.ReadAsStringAsync();
            }

            return response;
        }

        public async Task<HttpResponseMessage> CheckRoomAsync()
        {
            return await _httpClient.GetAsync(ControllerPath + $"check_room/{RoomId}");
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
            return await _httpClient.GetAsync(ControllerPath + $"check_move/{RoomId}");
        }

        public async Task<HttpResponseMessage> SendConfirmationAsync()
        {
            return await _httpClient.PostAsync(ControllerPath + $"send_confirmation/{RoomId}",
                true,
                new JsonMediaTypeFormatter());
        }

        public async Task<HttpResponseMessage> CheckConfirmationAsync()
        {
            return await _httpClient.GetAsync(ControllerPath + $"check_confirmation/{RoomId}");
        }

        public async Task<HttpResponseMessage> CheckRoundStateAsync()
        {
            return await _httpClient.GetAsync(ControllerPath + $"check_round_state/{RoomId}");
        }

        public async Task<HttpResponseMessage> SurrenderAsync()
        {
            return await _httpClient.GetAsync(ControllerPath + $"surrender/{RoomId}");
        }

        public async Task<HttpResponseMessage> ExitFromRoomAsync()
        {
            return await _httpClient.GetAsync(ControllerPath + $"exit/{RoomId}");
        }

        public async Task<HttpResponseMessage> GetResultsAsync()
        {
            return await _httpClient.GetAsync(ControllerPath + $"get_results/{RoomId}");
        }
    }
}
