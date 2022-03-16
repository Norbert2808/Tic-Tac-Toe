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

        private readonly IUserService _userService;

        private readonly ISettingsService _settingsService;

        private readonly IRoundMenuState _roundState;

        private readonly ILogger<RoomMenuState> _logger;

        public RoomMenuState(IRoundMenuState roundState,
            IGameService gameService,
            IUserService userService,
            ISettingsService settingsService,
            ILogger<RoomMenuState> logger)
        {
            _gameService = gameService;
            _userService = userService;
            _settingsService = settingsService;
            _roundState = roundState;
            _logger = logger;
        }

        public async Task InvokeMenuAsync()
        {
            LogInformationAboutClass(nameof(InvokeMenuAsync), "Execute method");

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

                var roomId = "";
                var isConnecting = false;
                RoomType type;

                try
                {
                    ConsoleHelper.ReadIntFromConsole(out var choose);

                    switch (choose)
                    {
                        case 1:
                            LogInformationAboutClass(nameof(InvokeMenuAsync), "Creating private room.");
                            type = RoomType.Private;
                            break;

                        case 2:
                            LogInformationAboutClass(nameof(InvokeMenuAsync),
                                "Connect to private room executed");
                            type = RoomType.Private;
                            TokenHandler(out roomId);
                            isConnecting = true;
                            break;

                        case 3:
                            LogInformationAboutClass(nameof(InvokeMenuAsync), "Creating public room.");
                            type = RoomType.Public;
                            break;

                        case 4:
                            LogInformationAboutClass(nameof(InvokeMenuAsync), "Creating practice room.");
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
                    _logger.LogError("Invalid input data: {Message}", ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "It's not a number!" },
                        ConsoleColor.Red);
                    _ = Console.ReadLine();
                    continue;
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError("Failed to connect with server: {Message}", ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "Failed to connect with server!" },
                        ConsoleColor.Red);
                    _ = Console.ReadLine();
                    continue;
                }
                catch (OverflowException ex)
                {
                    _logger.LogError("Number is very large: {Message}", ex.Message);
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
            LogInformationAboutClass(nameof(StartConnectionWithRoomAsync), "Creating room...");

            var settings = await _settingsService.ConfigureSettingsAsync(_userService.Login!, type,
                roomId, isConnecting);

            var startRoomResponse = await _gameService.StartRoomAsync(settings);

            LogInformationAboutClass(nameof(StartConnectionWithRoomAsync),
                $"Response {startRoomResponse.StatusCode}");

            switch (startRoomResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    await GetMessageForRoomAsync(type, startRoomResponse);
                    break;


                case HttpStatusCode.BadRequest:
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
                    await InvokeRoundStateAsync();
                    return;

                default:
                    break;
            }

            await WaitSecondPlayerAsync(message);
        }

        public async Task InvokeRoundStateAsync()
        {
            LogInformationAboutClass(nameof(InvokeRoundStateAsync), "Start practice room");
            await _roundState.InvokeMenuAsync();
        }

        public async Task ExitFromRoomAsync()
        {
            LogInformationAboutClass(nameof(ExitFromRoomAsync), "Exit from room");
            _ = await _gameService.ExitFromRoomAsync();
        }

        public async Task WaitSecondPlayerAsync(string message)
        {
            LogInformationAboutClass(nameof(WaitSecondPlayerAsync), "Wait second players...");

            while (true)
            {
                Console.Clear();
                ConsoleHelper.WriteInConsole(message,
                    ConsoleColor.Green);
                var checkRoomResponse = await _gameService.CheckRoomAsync();

                LogInformationAboutClass(nameof(WaitSecondPlayerAsync),
                    $"Response {checkRoomResponse.StatusCode}");

                switch (checkRoomResponse.StatusCode)
                {
                    case HttpStatusCode.OK:
                        await InvokeRoundStateAsync();
                        return;

                    case HttpStatusCode.NotFound:
                        var time = await checkRoomResponse.Content.ReadAsStringAsync();
                        ConsoleHelper.WriteInConsole($"Time: {time}\n", ConsoleColor.Red);
                        Thread.Sleep(3000);
                        break;

                    case HttpStatusCode.BadRequest:
                        Console.Clear();
                        var errorMsg = await checkRoomResponse.Content.ReadAsStringAsync();
                        ConsoleHelper.WriteInConsole(errorMsg, ConsoleColor.Red);
                        _ = Console.ReadLine();
                        return;

                    case HttpStatusCode.Conflict:
                        await ExitFromRoomAsync();
                        return;

                    default:
                        break;
                }
            }
        }

        public void LogInformationAboutClass(string methodName, string message)
        {
            _logger.LogInformation("{ClassName}::{MethodName}::{Message}",
                nameof(RoomMenuState), methodName, message);
        }
    }
}
