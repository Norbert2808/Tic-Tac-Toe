using System.Net;
using Microsoft.Extensions.Logging;
using TicTacToe.Client.DTO;
using TicTacToe.Client.Services;

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
            var response = await _statisticService.GetPrivateStatisticDto();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var statisticDto = await response.Content.ReadAsAsync<PrivateStatisticDto>();

                var mostUsedPosition = string.Join(" ", statisticDto.MostUsedPosition!);
                var mostUsedNumbers = string.Join(" ", statisticDto.MostUsedNumbers!);

                Console.Clear();
                ConsoleHelper.WriteInConsole("--- Private statistic ---\n", ConsoleColor.Cyan);
                ConsoleHelper.WriteInConsole($"Count of winning games - {statisticDto.Winnings}\n",
                    ConsoleColor.Green);
                ConsoleHelper.WriteInConsole($"Count of lost games - {statisticDto.Losses}\n",
                    ConsoleColor.Red);
                ConsoleHelper.WriteInConsole(new string[]
                {
                    $"Most used position: {mostUsedPosition}",
                    $"Most used numbers: {mostUsedNumbers}",
                    $"All time in game: {statisticDto.AllTimeInGame:dd\\:mm\\:ss}"
                }, ConsoleColor.Yellow);
                _ = Console.ReadLine();
            }
        }
    }
}
