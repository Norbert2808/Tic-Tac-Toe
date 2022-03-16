using System.Net;
using Microsoft.Extensions.Logging;
using TicTacToe.Client.Services;
using TicTacToe.Client.Tools;

namespace TicTacToe.Client.States.Impl
{
    internal class AuthorizationMenuState : IAuthorizationMenuState
    {
        private readonly IUserService _userService;

        private readonly IMainMenuState _mainMenuState;

        private readonly ILeaderMenuState _leaderMenu;

        private readonly ILogger<AuthorizationMenuState> _logger;

        public AuthorizationMenuState(IUserService userService,
            IMainMenuState mainMenuState,
            ILeaderMenuState leaderMenu,
            ILogger<AuthorizationMenuState> logger)
        {
            _userService = userService;
            _mainMenuState = mainMenuState;
            _leaderMenu = leaderMenu;
            _logger = logger;
        }

        public async Task InvokeMenuAsync()
        {
            LogInformationAboutClass(nameof(InvokeMenuAsync), "Execute method");

            while (true)
            {
                Console.Clear();
                Introduction();
                ConsoleHelper.WriteInConsole(new[]
                {
                    "Authorization Menu",
                    "Please choose action:",
                    "1 -- Login",
                    "2 -- Registration",
                    "3 -- LeaderBoard",
                    "0 -- Exit"
                }, ConsoleColor.Yellow);

                try
                {
                    ConsoleHelper.ReadIntFromConsole(out var choose);
                    LogInformationAboutClass(nameof(InvokeMenuAsync), $"User choose action: {choose}");
                    switch (choose)
                    {
                        case 1:
                            await ExecuteLoginAsync();
                            break;

                        case 2:
                            await ExecuteRegistrationAsync();
                            break;

                        case 3:
                            await ExecuteLeaderMenuAsync();
                            break;

                        case 0:
                            LogInformationAboutClass(nameof(InvokeMenuAsync),
                                "User left the program");
                            return;

                        default:
                            continue;
                    }
                }
                catch (FormatException ex)
                {
                    _logger.LogError("Exception invalid format::{Message}", ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "It's not a number!" },
                        ConsoleColor.Red);
                    _ = Console.ReadLine();
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError("The connection to the server is gone: {Message}", ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "Failed to connect with server!" },
                        ConsoleColor.Red);
                    _ = Console.ReadLine();
                }
                catch (OverflowException ex)
                {
                    _logger.LogError("Number is very large: {Message}", ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "Number is very large!" },
                        ConsoleColor.Red);
                    _ = Console.ReadLine();
                }
            }
        }

        private static void Introduction()
        {
            ConsoleHelper.WriteInConsole(new[]
            {
                ".-----. _         .-----.             .-----.            ",
                "`-. .-':_;        `-. .-'             `-. .-'            ",
                "  : :  .-. .--.     : : .--.   .--.     : : .--.  .--.   ",
                "  : :  : :'  ..'    : :' .; ; '  ..'    : :' .; :' '_.'  ",
                "  :_;  :_;`.__.'    :_;`.__,_;`.__.'    :_;`.__.'`.__.'  "
            }, ConsoleColor.Blue, "Welcome to Tic Tac Toe");
        }

        public async Task ExecuteLoginAsync()
        {
            LogInformationAboutClass(nameof(ExecuteLoginAsync),
                "Login.");

            ConsoleHelper.WriteInConsole(new[] { "Enter login:" }, ConsoleColor.Cyan, "");
            var login = Console.ReadLine();
            ConsoleHelper.WriteInConsole(new[] { "Enter password:" }, ConsoleColor.Cyan, "");
            var password = Console.ReadLine();

            var response = await _userService.LoginAsync(login!, password!);
            await ResponseHandlerAsync(response);
        }

        public async Task ExecuteRegistrationAsync()
        {
            LogInformationAboutClass(nameof(ExecuteRegistrationAsync),
                "Registration.");

            ConsoleHelper.WriteInConsole(new[] { "Enter login for registration:" },
                ConsoleColor.Cyan,
                "");
            var login = Console.ReadLine();

            ConsoleHelper.WriteInConsole(new[] { "Enter password for registration:" },
                ConsoleColor.Cyan,
                "");
            var password = Console.ReadLine();

            var response = await _userService.RegistrationAsync(login!, password!);
            await ResponseHandlerAsync(response);
        }

        public async Task ExecuteLeaderMenuAsync()
        {
            LogInformationAboutClass(nameof(ExecuteLeaderMenuAsync),
                $"Invoke {nameof(LeaderMenuState)} class and execute {nameof(InvokeMenuAsync)}");
            await _leaderMenu.InvokeMenuAsync();
        }

        private async Task ResponseHandlerAsync(HttpResponseMessage response)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();

            LogInformationAboutClass(nameof(ResponseHandlerAsync),
                $"Response: {response.StatusCode}");

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    Console.Clear();
                    await _mainMenuState.InvokeMenuAsync();
                    break;

                case HttpStatusCode.BadRequest:
                    ConsoleHelper.WriteInConsole(
                        new[] { "Login and password must be at least 6 and no more than 25 characters long." },
                        ConsoleColor.Red);
                    _ = Console.ReadLine();
                    break;

                case HttpStatusCode.NotFound:
                    ConsoleHelper.WriteInConsole(new[] { errorMessage }, ConsoleColor.Red);
                    _ = Console.ReadLine();
                    break;

                case HttpStatusCode.Conflict:
                    ConsoleHelper.WriteInConsole(new[] { errorMessage }, ConsoleColor.Red);
                    _ = Console.ReadLine();
                    break;
            }
        }

        public void LogInformationAboutClass(string methodName, string message)
        {
            _logger.LogInformation("{ClassName}::{MethodName}::{Message}",
                nameof(AuthorizationMenuState), methodName, message);
        }
    }
}
