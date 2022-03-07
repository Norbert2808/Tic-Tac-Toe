using System.Net;
using Microsoft.Extensions.Logging;
using TicTacToe.Client.DTO;
using TicTacToe.Client.Services;

namespace TicTacToe.Client.States.Impl
{
    internal class PrivateUserStatistic : IPrivateUserStatistic
    {
        private readonly IStatisticService _statisticService;

        private readonly ILogger<IPrivateUserStatistic> _logger;

        public PrivateUserStatistic(IStatisticService statisticService,
            ILogger<IPrivateUserStatistic> logger)
        {
            _statisticService = statisticService;
            _logger = logger;
        }

        public async Task InvokeMenuAsync()
        {
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
                    $"Count of lost games - {statisticDto.Winnings}",
                    $"Most used position: {mostUsedPosition}",
                    $"Most used numbers: {mostUsedNumbers}"
                }, ConsoleColor.Cyan);
                _ = Console.ReadLine();
            }
        }
    }
}
