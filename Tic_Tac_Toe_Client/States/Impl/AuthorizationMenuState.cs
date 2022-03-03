using System.Net;
using Microsoft.Extensions.Logging;
using TicTacToe.Client.Services;

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
            _logger.LogInformation("Class AuthorizationMenuState. InvokeAsync method");
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
                    _logger.LogInformation("User choose action.");
                    ConsoleHelper.ReadIntFromConsole(out var choose);
                    switch (choose)
                    {
                        case 1:
                            _logger.LogInformation("Execute login method");
                            await ExecuteLoginAsync();
                            break;

                        case 2:
                            _logger.LogInformation("Execute registration method");
                            await ExecuteRegistrationAsync();
                            break;

                        case 3:
                            await _leaderMenu.InvokeMenuAsync();
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
            ConsoleHelper.WriteInConsole(new[] { "Enter login:" }, ConsoleColor.Cyan, "");
            var login = Console.ReadLine();
            ConsoleHelper.WriteInConsole(new[] { "Enter password:" }, ConsoleColor.Cyan, "");
            var password = Console.ReadLine();

            var response = await _userService.LoginAsync(login!, password!);
            await ResponseHandlerAsync(response);
        }

        public async Task ExecuteRegistrationAsync()
        {
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

        private async Task ResponseHandlerAsync(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.Clear();
                await _mainMenuState.InvokeMenuAsync();
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogInformation("User blocked");
                ConsoleHelper.WriteInConsole(
                    new[] { "You are blocked! Please waiting 1 minute" },
                    ConsoleColor.Red);
                _ = Console.ReadLine();
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                _logger.LogInformation("Input invalid data. HttpStatus::BadRequest");
                ConsoleHelper.WriteInConsole(
                       new[] { "Login and password must be at least 6 symbol long" },
                       ConsoleColor.Red);
                _ = Console.ReadLine();
            }

            if (response.StatusCode is HttpStatusCode.NotFound
                or HttpStatusCode.Conflict)
            {
                _logger.LogInformation("User with such login already registered or" +
                                       " input invalid data. HttpStatus::NotFound or HttpStatus::Conflict");
                var errorMessage = await GetMessageFromResponseAsync(response);
                ConsoleHelper.WriteInConsole(new[] { errorMessage }, ConsoleColor.Red);
                _ = Console.ReadLine();
            }
        }

        public async Task<string> GetMessageFromResponseAsync(HttpResponseMessage response)
        {
            return await response.Content.ReadAsStringAsync();
        }
    }
}
