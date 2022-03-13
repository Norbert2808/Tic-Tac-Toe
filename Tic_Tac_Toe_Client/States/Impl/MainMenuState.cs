using System.Net;
using Microsoft.Extensions.Logging;
using TicTacToe.Client.Services;
using TicTacToe.Client.Tools;

namespace TicTacToe.Client.States.Impl
{
    internal class MainMenuState : IMainMenuState
    {
        private readonly IRoomMenuState _roomMenuState;

        private readonly IUserService _userService;

        private readonly IPrivateStatisticState _privateUserStatistic;

        private readonly ILogger<MainMenuState> _logger;

        public MainMenuState(IRoomMenuState roomMenuState,
            IUserService userService,
            IPrivateStatisticState privateUserStatistic,
            ILogger<MainMenuState> logger)
        {
            _roomMenuState = roomMenuState;
            _userService = userService;
            _privateUserStatistic = privateUserStatistic;
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
                    "Main Menu",
                    "Please choose action:",
                    "1 -- Start room",
                    "2 -- Private statistic",
                    "0 -- Logout"
                }, ConsoleColor.Cyan);

                try
                {
                    ConsoleHelper.ReadIntFromConsole(out var choose);
                    switch (choose)
                    {
                        case 1:
                            await ExecuteRoomMenuAsync();
                            break;

                        case 2:
                            await _privateUserStatistic.InvokeMenuAsync();
                            break;

                        case 0:
                            await LogoutAsync();
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
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "Failed to connect with server!" },
                        ConsoleColor.Red);
                    _ = Console.ReadLine();
                }
            }
        }

        public async Task ExecuteRoomMenuAsync()
        {
            _logger.LogInformation("Execute round menu state.");
            await _roomMenuState.InvokeMenuAsync();
        }

        public async Task LogoutAsync()
        {
            var response = await _userService.LogoutAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                _logger.LogInformation("MainMenuState::LogoutAsync::Response: Successful response 200");

                var message = await response.Content.ReadAsStringAsync();
                ConsoleHelper.WriteInConsole(
                    new[] { message }, ConsoleColor.Yellow);
                _ = Console.ReadLine();
            }
        }
    }
}
