using Microsoft.Extensions.Logging;
using TicTacToe.Client.Services;

namespace TicTacToe.Client.States.Impl
{
    internal class AuthorizationMenuState : IAuthorizationMenuState
    {
        private readonly IUserService _userService;

        private readonly IState _mainMenuState;
        
        private readonly IState _leaderMenu;

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

        public async Task InvokeAsync()
        {
            _logger.LogInformation("Class AuthorizationMenuState. InvokeAsync method");
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Authorization Menu");
                Console.WriteLine("Please choose action:");
                Console.WriteLine("1 -- Login");
                Console.WriteLine("2 -- Registration");
                Console.WriteLine("3 -- LeaderBoard");
                Console.WriteLine("0 -- Exit");
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
                            await _leaderMenu.InvokeAsync();
                            break;
                        
                        case 0:
                            return;
                    }
                }
                catch (FormatException ex)
                {
                    _logger.LogError(ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "It's not a number!" },
                        ConsoleColor.Red);
                    Console.ReadLine();
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "Failed to connect with server!" },
                        ConsoleColor.Red);
                    Console.ReadLine();
                }
            }
        }

        public async Task ExecuteLoginAsync()
        {
            Console.WriteLine("Enter login:");
            var login = Console.ReadLine();
            Console.WriteLine("Enter password:");
            var password = Console.ReadLine();

            var response = await _userService.LoginAsync(login!, password!);
            await ResponseHandlerAsync(response);
        }

        public async Task ExecuteRegistrationAsync()
        {
            Console.WriteLine("Enter login for registration:");
            var login = Console.ReadLine();
            Console.WriteLine("Enter password for registration:");
            var password = Console.ReadLine();

            var response = await _userService.RegistrationAsync(login!, password!);
            await ResponseHandlerAsync(response);
        }

        private async Task ResponseHandlerAsync(HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.Clear();
                await _mainMenuState.InvokeAsync();
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogInformation("User blocked");
                ConsoleHelper.WriteInConsole(
                       new[] { "You are blocked! Please waiting 1 minute" }, ConsoleColor.Red);
                Console.ReadLine();
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                _logger.LogInformation("Input invalid data. HttpStatus::BadRequest");
                ConsoleHelper.WriteInConsole(
                       new[] { "Login and password must be at least 6 symbol long" }, ConsoleColor.Red);
                Console.ReadLine();
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation("Input invalid data. HttpStatus::NotFound");
                var errorMessage = await GetMessageFromResponseAsync(response);
                ConsoleHelper.WriteInConsole(
                       new[] { errorMessage }, ConsoleColor.Red);
                Console.ReadLine();
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                _logger.LogInformation("User with such login already registered. HttpStatus::NotFound");
                var errorMessage = await GetMessageFromResponseAsync(response);
                ConsoleHelper.WriteInConsole(
                    new[] { errorMessage }, ConsoleColor.Red);
                Console.ReadLine();
            }
        }

        public async Task<string> GetMessageFromResponseAsync(HttpResponseMessage response)
        {
            return await response.Content.ReadAsStringAsync();
        }
    }
}
