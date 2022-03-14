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
            _logger.LogInformation("LeaderMenuState::InvokeMenuAsync");

            while (true)
            {
                _logger.LogInformation("LeaderMenuState::InvokeMenuAsync::User choose action.");
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
                }, ConsoleColor.Cyan, "");

                try
                {
                    ConsoleHelper.ReadIntFromConsole(out var choose);
                    SortingType type;
                    switch (choose)
                    {
                        case 1:
                            _logger.LogInformation("User chose: Type::Winnings");
                            type = SortingType.Winnings;
                            await ShowLeadersStatistic(type);
                            break;

                        case 2:
                            _logger.LogInformation("User chose: Type::Losses");
                            type = SortingType.Losses;
                            await ShowLeadersStatistic(type);
                            break;

                        case 3:
                            _logger.LogInformation("User chose: Type::WinRate");
                            type = SortingType.WinRate;
                            await ShowLeadersStatistic(type);
                            break;

                        case 4:
                            _logger.LogInformation("User chose: Type::Room");
                            type = SortingType.Rooms;
                            await ShowLeadersStatistic(type);
                            break;

                        case 5:
                            _logger.LogInformation("User chose: Type::Time");
                            type = SortingType.Time;
                            await ShowLeadersStatistic(type);
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

        public async Task ShowLeadersStatistic(SortingType type)
        {
            var responce = await _statisticService.GetLeadersStatisticAsync(type);
            if (responce.StatusCode == HttpStatusCode.OK)
            {
                var leaders = await responce.Content.ReadAsAsync<List<LeaderStatisticDto>>();
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
