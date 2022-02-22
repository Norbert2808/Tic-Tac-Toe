using System.Globalization;
using System.Text;
using Tic_Tac_Toe.Client.Services;

namespace Tic_Tac_Toe.Client.States
{
    internal class AuthorizationMenuState : IState
    {
        private readonly UserService _userService;

        private readonly IState _mainMenuState;

        public AuthorizationMenuState(UserService userService, IState mainMenuState)
        {
            _userService = userService;
            _mainMenuState = mainMenuState;
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
                    //ToDo
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine("Failed to connect with server!");
                    Console.ReadLine();
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
                throw new NotImplementedException();
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new NotImplementedException();
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
