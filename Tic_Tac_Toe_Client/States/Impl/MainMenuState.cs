using System.Net;
using Microsoft.Extensions.Logging;
using TicTacToe.Client.Services;

namespace TicTacToe.Client.States.Impl
{
    internal class MainMenuState : IMainMenuState
    {
        private readonly IState _gameMenuState;

        private readonly IUserService _userService;
        
        private readonly ILogger<IMainMenuState> _logger;

        public MainMenuState(IGameMenuState gameMenuState,
            IUserService userService,
            ILogger<IMainMenuState> logger)
        {
            _gameMenuState = gameMenuState;
            _userService = userService;
            _logger = logger;
        }

        public async Task InvokeMenuAsync()
        {
            _logger.LogInformation("Class MainMenuState. InvokeAsync method");
            
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Main Menu");
                Console.WriteLine("Please choose action:");
                Console.WriteLine("1 -- Start game");
                Console.WriteLine("2 -- Private statistic");
                Console.WriteLine("0 -- Logout");
                try
                {
                    ConsoleHelper.ReadIntFromConsole(out var choose);
                    switch (choose)
                    {
                        case 1:
                            await ExecuteGameMenuAsync();
                            break;

                        case 2:
                            
                            break;

                        case 0:
                            await LogoutAsync();
                            return;
                    }
                }
                catch (FormatException ex)
                {
                    _logger.LogError(ex.Message);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "Failed to connect with server!" },
                        ConsoleColor.Red);
                    Console.ReadLine();
                    return;
                }
            }
        }

        public async Task ExecuteGameMenuAsync()
        {
            _logger.LogInformation("Execute game menu state.");
            await _gameMenuState.InvokeMenuAsync();
        }

        public async Task LogoutAsync()
        {
            var response = await _userService.LogoutAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var message = await GetMessageFromResponseAsync(response);
                ConsoleHelper.WriteInConsole(
                    new[] { message }, ConsoleColor.Yellow);
                Console.ReadLine();
            }
        }

        public async Task<string> GetMessageFromResponseAsync(HttpResponseMessage response)
        {
            return await response.Content.ReadAsStringAsync();
        }
    }
}
