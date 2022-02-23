using Microsoft.Extensions.Logging;

namespace TicTacToe.Client.States
{
    internal class MainMenuState : IState
    {
        private readonly IState _gameMenuState;
        
        private readonly ILogger<MainMenuState> _logger;

        public MainMenuState(GameMenuState gameMenuState, ILogger<MainMenuState> logger)
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
                Console.WriteLine("1 -- Private statistic");
                Console.WriteLine("3 -- Logout");
                try
                {
                    ConsoleHelper.ReadIntFromConsole(out var choose);
                    switch (choose)
                    {
                        case 1:
                            await _gameMenuState.InvokeAsync();
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
    }
}
