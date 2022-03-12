using System.Net;
using Microsoft.Extensions.Logging;
using TicTacToe.Client.Enums;
using TicTacToe.Client.Services;

namespace TicTacToe.Client.States.Impl
{
    public class RoomMenuState : IRoomMenuState
    {
        private readonly IGameService _gameService;

        private readonly IRoundMenuState _roundState;

        private readonly ILogger<RoomMenuState> _logger;

        public RoomMenuState(IRoundMenuState roundState,
            IGameService gameService,
            ILogger<RoomMenuState> logger)
        {
            _gameService = gameService;
            _roundState = roundState;
            _logger = logger;
        }

        public async Task InvokeMenuAsync()
        {
            _logger.LogInformation("Class MainMenuState. InvokeAsync method");

            while (true)
            {
                Console.Clear();
                ConsoleHelper.WriteInConsole(new[]
                {
                    "Room Menu",
                    "Please choose room:",
                    "1 -- Create private room",
                    "2 -- Connect to private room",
                    "3 -- Public room",
                    "4 -- Practice room",
                    "0 -- Close"
                }, ConsoleColor.Cyan);

                RoomType type = default;
                var roomId = "";
                var isConnecting = false;
                try
                {
                    ConsoleHelper.ReadIntFromConsole(out var choose);
                    switch (choose)
                    {
                        case 1:
                            _logger.LogInformation("Creating private room.");
                            type = RoomType.Private;
                            break;

                        case 2:
                            _logger.LogInformation("Connect to private room executed");
                            type = RoomType.Private;
                            TokenHandler(out roomId);
                            isConnecting = true;
                            break;

                        case 3:
                            _logger.LogInformation("Creating public room.");
                            type = RoomType.Public;
                            break;

                        case 4:
                            _logger.LogInformation("Creating practice room.");
                            type = RoomType.Practice;
                            break;

                        case 0:
                            return;

                        default:
                            continue;
                    }
                }
                catch (FormatException ex)
                {
                    _logger.LogError(ex.Message);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "Failed to connect with server!" },
                        ConsoleColor.Red);
                    _ = Console.ReadLine();
                }

                await StartConnectionWithRoomAsync(type, roomId!, isConnecting);
            }
        }

        private static void TokenHandler(out string? token)
        {
            ConsoleHelper.WriteInConsole(new[] { "Please enter token:" },
                ConsoleColor.Yellow, "");
            token = Console.ReadLine();
        }

        public async Task StartConnectionWithRoomAsync(RoomType type, string roomId, bool isConnecting)
        {
            _logger.LogInformation("Creating room.");

            var startRoomResponse = await _gameService.StartRoomAsync(type, roomId, isConnecting);

            var message = Array.Empty<string>();

            if (startRoomResponse.StatusCode == HttpStatusCode.OK)
            {
                _logger.LogInformation("RoomMenuState::StartConnectionWithRoomAsync::Successful response 200");

                if (type == RoomType.Public)
                {
                    message = new[]
                    {
                        "Room was found! Please, be wait when your" +
                        " opponent will entering."
                    };
                }

                if (type == RoomType.Private)
                {
                    message = new[]
                    {
                        "Your private token:" +
                        $"{await startRoomResponse.Content.ReadAsStringAsync()}",
                        "Please, be wait when your opponent will entering."
                    };
                }

                if (type == RoomType.Practice)
                {
                    _logger.LogInformation("RoomMenuState::StartConnectionWithRoomAsync::Start practice room");
                    await _roundState.InvokeMenuAsync();
                    return;
                }

                await WaitSecondPlayerAsync(message);
            }
            if (startRoomResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                _logger.LogInformation("RoomMenuState::StartConnectionWithRoomAsync::Bad request 400");
                var errorMessage = await startRoomResponse.Content.ReadAsStringAsync();
                ;
                ConsoleHelper.WriteInConsole(new[] { errorMessage }, ConsoleColor.Red);
                _ = Console.ReadLine();
            }
        }

        public async Task WaitSecondPlayerAsync(string[] message)
        {
            _logger.LogInformation("Waiting second player.");
            while (true)
            {
                Console.Clear();
                ConsoleHelper.WriteInConsole(message,
                    ConsoleColor.Green, "");
                var checkRoomResponse = await _gameService.CheckRoomAsync();

                if (checkRoomResponse.StatusCode == HttpStatusCode.OK)
                {
                    _logger.LogInformation("RoomMenuState::WaitSecondPlayerAsync::Successful response 200");
                    await _roundState.InvokeMenuAsync();
                    return;
                }

                if (checkRoomResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("RoomMenuState::WaitSecondPlayerAsync::Response: NotFound 404");
                    var time = await checkRoomResponse.Content.ReadAsAsync<string[]>();
                    ConsoleHelper.WriteInConsole(new[] { $"Time: {time[0]}" }, ConsoleColor.Red, "");
                    Thread.Sleep(3000);
                }

                if (checkRoomResponse.StatusCode == HttpStatusCode.Conflict)
                {
                    _logger.LogInformation("RoomMenuState::WaitSecondPlayerAsync::Response: Conflict 409");
                    _ = await _gameService.ExitFromRoomAsync();
                    return;
                }
            }
        }
    }
}
