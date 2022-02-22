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
                    var choose = int.Parse(Console.ReadLine());
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
            throw new NotImplementedException();
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

            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {

            }
        }
    }
}
