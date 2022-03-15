using System.Net;
using Microsoft.Extensions.Logging;
using TicTacToe.Client.Enums;
using TicTacToe.Client.Services;
using TicTacToe.Client.Tools;

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
                    ConsoleHelper.WriteInConsole(new[] { "It's not a number!" },
                        ConsoleColor.Red);
                    _ = Console.ReadLine();
                    continue;
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "Failed to connect with server!" },
                        ConsoleColor.Red);
                    _ = Console.ReadLine();
                    continue;
                }
                catch (OverflowException ex)
                {
                    _logger.LogError(ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "Number is very large!" },
                        ConsoleColor.Red);
                    _ = Console.ReadLine();
                    continue;
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

            switch (startRoomResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    _logger.LogInformation("RoomMenuState::StartConnectionWithRoomAsync::Successful response 200");
                    await GetMessageForRoomAsync(type, startRoomResponse);
                    break;


                case HttpStatusCode.BadRequest:
                    _logger.LogInformation("RoomMenuState::StartConnectionWithRoomAsync::Bad request 400");
                    var errorMessage = await startRoomResponse.Content.ReadAsStringAsync();
                    ConsoleHelper.WriteInConsole(new[] { errorMessage }, ConsoleColor.Red);
                    _ = Console.ReadLine();
                    break;

                default:
                    break;
            }
        }

        private async Task GetMessageForRoomAsync(RoomType type, HttpResponseMessage startRoomResponse)
        {
            var message = string.Empty;

            switch (type)
            {
                case RoomType.Public:
                    message = "Room was found! Please, be wait when your" +
                    " opponent will entering.\n";
                    break;

                case RoomType.Private:
                    message = "Your private token:" +
                    $"{await startRoomResponse.Content.ReadAsStringAsync()}\n" +
                    "Please, be wait when your opponent will entering.\n";
                    break;

                case RoomType.Practice:
                    _logger.LogInformation("RoomMenuState::StartConnectionWithRoomAsync::Start practice room");
                    await _roundState.InvokeMenuAsync();
                    return;

                default:
                    break;
            }

            await WaitSecondPlayerAsync(message);
        }

        public async Task WaitSecondPlayerAsync(string message)
        {
            _logger.LogInformation("Waiting second player.");
            while (true)
            {
                Console.Clear();
                ConsoleHelper.WriteInConsole(message,
                    ConsoleColor.Green);
                var checkRoomResponse = await _gameService.CheckRoomAsync();

                switch (checkRoomResponse.StatusCode)
                {
                    case HttpStatusCode.OK:
                        _logger.LogInformation("RoomMenuState::WaitSecondPlayerAsync::Successful response 200");
                        await _roundState.InvokeMenuAsync();
                        return;

                    case HttpStatusCode.NotFound:
                        _logger.LogInformation("RoomMenuState::WaitSecondPlayerAsync::Response: NotFound 404");
                        var time = await checkRoomResponse.Content.ReadAsStringAsync();
                        ConsoleHelper.WriteInConsole($"Time: {time}\n", ConsoleColor.Red);
                        Thread.Sleep(3000);
                        break;

                    case HttpStatusCode.BadRequest:
                        _logger.LogInformation("RoomMenuState::WaitSecondPlayerAsync::Response: BadRequest 400");
                        Console.Clear();
                        var errorMsg = await checkRoomResponse.Content.ReadAsStringAsync();
                        ConsoleHelper.WriteInConsole(errorMsg, ConsoleColor.Red);
                        _ = Console.ReadLine();
                        return;

                    case HttpStatusCode.Conflict:
                        _logger.LogInformation("RoomMenuState::WaitSecondPlayerAsync::Response: Conflict 409");
                        _ = await _gameService.ExitFromRoomAsync();
                        return;

                    default:
                        break;
                }
            }
        }
    }
}
