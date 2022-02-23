using Microsoft.Extensions.Logging;

namespace TicTacToe.Client.States.Impl
{
    internal class MainMenuState : IMainMenuState
    {
        private readonly IState _gameMenuState;
        
        private readonly ILogger<IMainMenuState> _logger;

        public MainMenuState(IGameMenuState gameMenuState, ILogger<IMainMenuState> logger)
        {
            _gameMenuState = gameMenuState;
            _logger = logger;
        }

        public async Task InvokeAsync()
        {
            _logger.LogInformation("Class MainMenuState. InvokeAsync method");
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Main Menu");
                Console.WriteLine("Please choose action:");
                Console.WriteLine("1 -- Start game");
                Console.WriteLine("2 -- Private statistic");
                Console.WriteLine("3 -- Logout");
                try
                {
                    ConsoleHelper.ReadIntFromConsole(out var choose);
                    switch (choose)
                    {
                        case 1:
                            await ExecuteGameMenu();
                            break;

                        case 2:
                            
                            break;

                        case 3:
                            return;
                    }
                }
                catch (FormatException ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
        }

        public async Task ExecuteGameMenu()
        {
            await _gameMenuState.InvokeAsync();
        }
    }
}
