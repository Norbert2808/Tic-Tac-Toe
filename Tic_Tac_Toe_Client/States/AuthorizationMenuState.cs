using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using Tic_Tac_Toe.Client.Services;

namespace Tic_Tac_Toe.Client.States
{
    internal class AuthorizationMenuState : IState
    {
        private readonly UserService _userService;

        private readonly IState _mainMenuState;
        
        private readonly IState _leaderMenu;

        private readonly ILogger<AuthorizationMenuState> _logger;

        public AuthorizationMenuState(UserService userService, 
            MainMenuState mainMenuState,
            LeaderMenuState leaderMenu,
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
                Console.WriteLine("4 -- Exit");
                try
                {
                    _logger.LogInformation("User choose action.");
                    ConsoleHelper.ReadIntFromConsole(out var choose);
                    switch (choose)
                    {
                        case 1:
                            await ExecuteLoginAsync();
                            break;
                        
                        case 2:
                            
                            await ExecuteRegistrationAsync();
                            break;

                        case 3:
                            await _leaderMenu.InvokeAsync();
                            break;
                        
                        case 4:
                            return;
                    }
                }
                catch (FormatException ex)
                {
                    _logger.LogError(ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "It's not a number!" },
                        ConsoleColor.Red);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "Failed to connect with server!" },
                        ConsoleColor.Red);
                }
            }
        }

        private async Task ExecuteLoginAsync()
        {
            _logger.LogInformation("Invoke login method");
            
            Console.WriteLine("Enter login:");
            var login = Console.ReadLine();
            Console.WriteLine("Enter password:");
            var password = Console.ReadLine();

            var response = await _userService.LoginAsync(login!, password!);
            await ResponseHandlerAsync(response);
        }

        private async Task ExecuteRegistrationAsync()
        {
            _logger.LogInformation("Invoke registration method"); 
            
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
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                _logger.LogInformation("Input data not valid.");
                ConsoleHelper.WriteInConsole(
                       new[] { "Login and password must be at least 6 symbol long" }, ConsoleColor.Red);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation("Input invalid data.");
                var errorMessage = GetMessageFromResponse(response);
                ConsoleHelper.WriteInConsole(
                       new[] { errorMessage }, ConsoleColor.Red);
            }
        }

        private string GetMessageFromResponse(HttpResponseMessage response)
        {
            var stream = response.Content.ReadAsStream();
            stream.Position = 0;
            using var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }
}
