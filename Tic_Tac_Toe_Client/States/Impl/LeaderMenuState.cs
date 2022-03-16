using System.Net;
using Microsoft.Extensions.Logging;
using TicTacToe.Client.DTO;
using TicTacToe.Client.Enums;
using TicTacToe.Client.Services;
using TicTacToe.Client.Tools;

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
            LogInformationAboutClass(nameof(InvokeMenuAsync), "Execute method");

            while (true)
            {
                Console.Clear();
                ConsoleHelper.WriteInConsole(new[]
                {
                    "LeaderBoard menu",
                    "There are users who have more than 10 rounds",
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

                    LogInformationAboutClass(nameof(InvokeMenuAsync), $"{nameof(SortingType)} :" +
                        ((SortingType)Enum.Parse(typeof(SortingType),
                            choose.ToString())));

                    SortingType type;
                    switch (choose)
                    {
                        case 1:
                            type = SortingType.Winnings;
                            await ShowLeadersStatisticAsync(type);
                            break;

                        case 2:
                            type = SortingType.Losses;
                            await ShowLeadersStatisticAsync(type);
                            break;

                        case 3:
                            type = SortingType.WinRate;
                            await ShowLeadersStatisticAsync(type);
                            break;

                        case 4:
                            type = SortingType.Rooms;
                            await ShowLeadersStatisticAsync(type);
                            break;

                        case 5:
                            type = SortingType.Time;
                            await ShowLeadersStatisticAsync(type);
                            break;

                        case 0:
                            return;

                        default:
                            continue;
                    }
                }
                catch (FormatException ex)
                {
                    _logger.LogError("It's not a number: {Message}", ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "It's not a number!" }, ConsoleColor.Red);
                    _ = Console.ReadLine();
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError("Failed to connect with server: {Message}", ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "Failed to connect with server!" },
                        ConsoleColor.Red);
                    _ = Console.ReadLine();
                }
                catch (OverflowException ex)
                {
                    _logger.LogError("Number is very large: {Message}", ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "Number is very large!" },
                        ConsoleColor.Red);
                    _ = Console.ReadLine();
                }
            }
        }

        public void LogInformationAboutClass(string methodName, string message)
        {
            _logger.LogInformation("{ClassName}::{MethodName}::{Message}",
                nameof(LeaderMenuState), methodName, message);
        }

        public async Task ShowLeadersStatisticAsync(SortingType type)
        {
            var response = await _statisticService.GetLeadersStatisticAsync(type);

            LogInformationAboutClass(nameof(ShowLeadersStatisticAsync), $"Response: {response.StatusCode}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var leaders = await response.Content.ReadAsAsync<List<LeaderStatisticDto>>();
                var count = 1;

                Console.Clear();
                var consoleTable = leaders.ToStringTable(
                    new[] { "№", "LOGIN", "WINNINGS", "LOSSES", "WINRATE", "ROOMS", "TIME" },
                    leader => count++,
                    leader => leader.Login,
                    leader => leader.Winnings,
                    leader => leader.Losses,
                    leader => $"{leader.WinRate}%",
                    leader => leader.RoomsNumber,
                    leader => $"{leader.Time:dd\\:mm\\:ss}");

                ConsoleHelper.WriteInConsole(consoleTable, ConsoleColor.DarkYellow);

                Console.WriteLine("\nPlease press to continue");
                _ = Console.ReadLine();
            }
        }
    }
}
