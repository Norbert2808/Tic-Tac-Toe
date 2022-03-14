using System.Net;
using Microsoft.Extensions.Logging;
using TicTacToe.Client.DTO;
using TicTacToe.Client.Services;
using TicTacToe.Client.Tools;

namespace TicTacToe.Client.States.Impl
{
    internal class PrivateStatisticState : IPrivateStatisticState
    {
        private readonly IStatisticService _statisticService;

        private readonly ILogger<IPrivateStatisticState> _logger;

        public PrivateStatisticState(IStatisticService statisticService,
            ILogger<IPrivateStatisticState> logger)
        {
            _statisticService = statisticService;
            _logger = logger;
        }

        public async Task InvokeMenuAsync()
        {
            _logger.LogInformation("PrivateStatisticState::Invoke private statistic");

            while (true)
            {
                Console.Clear();
                ConsoleHelper.WriteInConsole(new[]
                {
                    "Private statistic",
                    "Please choose action:",
                    "1 -- All private statistic",
                    "2 -- Wins and losses in a given time interval",
                    "0 -- Close"
                }, ConsoleColor.Cyan);

                try
                {
                    ConsoleHelper.ReadIntFromConsole(out var choose);
                    switch (choose)
                    {
                        case 1:
                            await GetPrivateStatistic();
                            break;

                        case 2:

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
                    ConsoleHelper.WriteInConsole(new[] { "It's not a number!" }, ConsoleColor.Red);
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

        private async Task GetPrivateStatistic()
        {
            var response = await _statisticService.GetPrivateStatisticAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var statisticDto = await response.Content.ReadAsAsync<PrivateStatisticDto>();

                var mostUsedPosition = string.Join(" ", statisticDto.MostUsedPosition!);
                var mostUsedNumbers = string.Join(" ", statisticDto.MostUsedNumbers!);

                Console.Clear();
                ConsoleHelper.WriteInConsole("--- Private statistic ---\n\n", ConsoleColor.Cyan);

                ConsoleHelper.WriteInConsole($"Number of winning games - {statisticDto.Winnings}\n",
                    ConsoleColor.Green);
                ConsoleHelper.WriteInConsole($"Number of lost games - {statisticDto.Losses}\n",
                    ConsoleColor.Red);
                ConsoleHelper.WriteInConsole($"Total number of rooms - {statisticDto.TotalNumberOfRooms}\n",
                    ConsoleColor.Blue);
                ConsoleHelper.WriteInConsole($"Total number of moves - {statisticDto.TotalNumberOfMoves}\n\n",
                    ConsoleColor.Blue);

                ConsoleHelper.WriteInConsole(new string[]
                {
                    statisticDto.MostPositionCount == 0 ? "No the most used position" :
                    $"Most used position: {mostUsedPosition}, count of use - {statisticDto.MostPositionCount}",
                    statisticDto.MostNumbersCount == 0 ? "No the most used numbers" :
                    $"Most used numbers: {mostUsedNumbers}, count of use - {statisticDto.MostNumbersCount}",
                    $"All time in game: {statisticDto.AllTimeInGame:dd\\:mm\\:ss}\n",
                }, ConsoleColor.Yellow);
                _ = Console.ReadLine();
            }
        }
    }
}
