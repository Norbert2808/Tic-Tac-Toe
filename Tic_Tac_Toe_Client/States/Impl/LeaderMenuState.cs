using Microsoft.Extensions.Logging;
using TicTacToe.Client.Services;

namespace TicTacToe.Client.States.Impl
{
    public class LeaderMenuState : ILeaderMenuState
    {
        private readonly IStatisticService _statisticService;

        private readonly ILogger<LeaderMenuState> _logger;

        public LeaderMenuState(IStatisticService statisticService, ILogger<LeaderMenuState> logger)
        {
            _statisticService = statisticService;
            _logger = logger;
        }

        public async Task InvokeMenuAsync()
        {
            while (true)
            {
                Console.Clear();
                ConsoleHelper.WriteInConsole(new[]
                {
                "LeaderBoard menu",
                "Users who have more than 10 rounds",
                "Please choose type of sorting or close:",
                "1 -- By number of wins",
                "2 -- By number of lose",
                "3 -- By win rate",
                "4 -- By number of rooms",
                "5 -- By times",
                "0 -- Close"
            }, ConsoleColor.Cyan);

                try
                {
                    ConsoleHelper.ReadIntFromConsole(out var choose);
                    switch (choose)
                    {
                        case 1:
                            break;

                        case 2:
                            break;

                        case 0:
                            return;

                        default:
                            break;
                    }
                }
                catch (FormatException ex)
                {
                    _logger.LogError(ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "It's not a number!" }, ConsoleColor.DarkRed);
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

        public Task<string> GetMessageFromResponseAsync(HttpResponseMessage response)
        {
            throw new NotImplementedException();
        }
    }
}
