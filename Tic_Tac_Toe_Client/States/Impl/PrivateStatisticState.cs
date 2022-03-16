using System.Globalization;
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
                    "2 -- Private statistic in a given time interval",
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
                            await GetPrivateStatisticInTimeInterval();
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
                catch (OverflowException ex)
                {
                    _logger.LogError(ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "Number is very large!" },
                        ConsoleColor.Red);
                    _ = Console.ReadLine();
                }
            }
        }

        public async Task GetPrivateStatistic()
        {
            var response = await _statisticService.GetPrivateStatisticAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var statisticDto = await response.Content.ReadAsAsync<PrivateStatisticDto>();

                Console.Clear();
                DrawPrivateStatistic(statisticDto);
            }
        }

        public async Task GetPrivateStatisticInTimeInterval()
        {
            var timeInterval = GetTimeIntervalFromUser();

            var response = await _statisticService.GetPrivateStatisticInTimeIntervalAsync(timeInterval);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var statisticDto = await response.Content.ReadAsAsync<PrivateStatisticDto>();

                Console.Clear();
                ConsoleHelper.WriteInConsole(
                    $"Time interval: [{timeInterval.StartTime} - {timeInterval.EndTime}]\n",
                    ConsoleColor.Cyan);
                DrawPrivateStatistic(statisticDto);
            }
        }

        private void DrawPrivateStatistic(PrivateStatisticDto statisticDto)
        {
            var mostUsedPosition = string.Join(" ", statisticDto.MostUsedPosition!);
            var mostUsedNumbers = string.Join(" ", statisticDto.MostUsedNumbers!);

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

        private TimeIntervalDto GetTimeIntervalFromUser()
        {
            DateTime start;
            DateTime end;

            while (true)
            {
                try
                {
                    var parseFormat = "dd.MM.yyyy HH:mm";

                    Console.Clear();
                    ConsoleHelper.WriteInConsole($"Enter start time in format \"{parseFormat}\": ",
                        ConsoleColor.DarkGreen);
                    var startTime = Console.ReadLine();
                    ConsoleHelper.WriteInConsole($"Enter end time in format \"{parseFormat}\": ",
                        ConsoleColor.DarkGreen);
                    var endTime = Console.ReadLine();

                    if (!DateTime.TryParseExact(
                        startTime!, parseFormat, CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out start))
                    {
                        throw new FormatException(startTime);
                    }

                    if (!DateTime.TryParseExact(
                        endTime!, parseFormat, CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out end))
                    {
                        throw new FormatException(endTime);
                    }
                }
                catch (FormatException ex)
                {
                    _logger.LogError($"String '{ex.Message}' was not recognized as a valid DateTime.");
                    ConsoleHelper.WriteInConsole(
                        new[] { $"String '{ex.Message}' was not recognized as a valid DateTime." },
                        ConsoleColor.Red);
                    _ = Console.ReadLine();
                    continue;
                }

                if (end <= start)
                {
                    _logger.LogInformation("Start time cannot be longer or equal than end time");
                    ConsoleHelper.WriteInConsole(
                        new[] { "Start time cannot be longer or equal than end time" },
                        ConsoleColor.Red);
                    _ = Console.ReadLine();
                    continue;
                }

                return new(start, end);
            }
        }
    }
}
