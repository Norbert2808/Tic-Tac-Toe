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
                ConsoleHelper.WriteInConsole("--- Private statistic ---\n\n", ConsoleColor.Cyan);
                ConsoleHelper.WriteInConsole($"Count of winning games - {statisticDto.Winnings}\n",
                    ConsoleColor.Green);
                ConsoleHelper.WriteInConsole($"Count of lost games - {statisticDto.Losses}\n\n",
                    ConsoleColor.Red);
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
