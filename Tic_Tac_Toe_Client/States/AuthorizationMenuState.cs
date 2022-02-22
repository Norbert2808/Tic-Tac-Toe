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

        private readonly ILogger<AuthorizationMenuState> _logger;

        public AuthorizationMenuState(UserService userService, 
            IState mainMenuState,
            ILogger<AuthorizationMenuState> logger)
        {
            _userService = userService;
            _mainMenuState = mainMenuState;
            _logger = logger;
        }

        public async Task InvokeAsync()
        {
            while (true)
            {
                Console.WriteLine("Authorization Menu");
                Console.WriteLine("Please choose action:");
                Console.WriteLine("1 -- Login");
                Console.WriteLine("2 -- Registration");
                Console.WriteLine("3 -- Close");
                try
                {
                    var choose = Convert.ToInt32(Console.ReadLine(), CultureInfo.CurrentCulture);
                    switch (choose)
                    {
                        case 1:
                            await ExecuteLoginAsync();
                            break;
                        case 2:
                            await ExecuteRegistrationAsync();
                            break;

                        case 3:
                            return;
                    }
                }
                catch (FormatException ex)
                {
                    ConsoleHelper.WriteInConsole(new string[] { "It's not a number!" }, ConsoleColor.Red);
                }
                catch (HttpRequestException ex)
                {
                    ConsoleHelper.WriteInConsole(
                        new string[] { "Failed to connect with server!" }, ConsoleColor.Red);
                }

                Console.Clear();
            }
        }

        private async Task ExecuteLoginAsync()
        {
            Console.WriteLine("Enter login:");
            var login = Console.ReadLine();
            Console.WriteLine("Enter password:");
            var password = Console.ReadLine();

            var response = await _userService.LoginAsync(login, password);
            await ResponseHandlerAsync(response);
        }

        private async Task ExecuteRegistrationAsync()
        {
            Console.WriteLine("Enter login for registration:");
            var login = Console.ReadLine();
            Console.WriteLine("Enter password for registration:");
            var password = Console.ReadLine();

            var response = await _userService.RegistrationAsync(login, password);
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
                ConsoleHelper.WriteInConsole(
                       new string[] { "You are blocked! Please waiting 1 minute" }, ConsoleColor.Red);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                ConsoleHelper.WriteInConsole(
                       new string[] { "Login and password must be at least 6 symbol long" }, ConsoleColor.Red);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var errorMessage = GetMessageFromResponse(response);
                ConsoleHelper.WriteInConsole(
                       new string[] { errorMessage }, ConsoleColor.Red);
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
