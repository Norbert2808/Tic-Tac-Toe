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
            _logger.LogInformation("Invoke private statistic");
            var response = await _statisticService.GetPrivateStatisticDto();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var statisticDto = await response.Content.ReadAsAsync<PrivateStatisticDto>();

                var mostUsedPosition = string.Join(" ", statisticDto.MostUsedPosition!);
                var mostUsedNumbers = string.Join(" ", statisticDto.MostUsedNumbers!);

                Console.Clear();
                ConsoleHelper.WriteInConsole(new string[]
                {
                    "--- Private statistic ---",
                    $"Count of winning games - {statisticDto.Winnings}",
                    $"Count of lost games - {statisticDto.Losses}",
                    $"Most used position: {mostUsedPosition}",
                    $"Most used numbers: {mostUsedNumbers}"
                }, ConsoleColor.Cyan);
                _ = Console.ReadLine();
            }
        }
    }
}
