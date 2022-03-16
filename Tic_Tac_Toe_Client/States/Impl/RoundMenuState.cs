using System.Net;
using Microsoft.Extensions.Logging;
using TicTacToe.Client.DTO;
using TicTacToe.Client.Services;
using TicTacToe.Client.Tools;

namespace TicTacToe.Client.States.Impl
{
    public class RoundMenuState : IRoundMenuState
    {
        private readonly IGameState _gameState;

        private readonly IGameService _gameService;

        private readonly ILogger<RoundMenuState> _logger;

        public RoundMenuState(IGameService gameService,
            IGameState gameState,
            ILogger<RoundMenuState> logger)
        {
            _gameService = gameService;
            _gameState = gameState;
            _logger = logger;
        }

        public async Task InvokeMenuAsync()
        {
            LogInformationAboutClass(nameof(InvokeMenuAsync), "Execute method");

            while (true)
            {
                try
                {
                    Console.Clear();
                    await ShowEnemyBarAsync();
                    ConsoleHelper.WriteInConsole(new[]
                    {
                        "1 -- Start new round",
                        "0 -- Exit"
                    }, ConsoleColor.Yellow);

                    ConsoleHelper.ReadIntFromConsole(out var choose);
                    switch (choose)
                    {
                        case 1:
                            if (await WaitingStartGame())
                                continue;
                            return;

                        case 0:
                            await ExitAsync();
                            return;

                        default:
                            continue;
                    }
                }
                catch (FormatException ex)
                {
                    _logger.LogError("Exception invalid format::{Message}", ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "It's not a number!" },
                        ConsoleColor.Red);
                    _ = Console.ReadLine();
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError("The connection to the server is gone: {Message}", ex.Message);
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
                nameof(RoundMenuState), methodName, message);
        }

        public async Task<bool> WaitingStartGame()
        {
            Console.Clear();
            var responseSendConfirmation = await _gameService.SendConfirmationAsync();
            LogInformationAboutClass(nameof(WaitingStartGame),
                $"Response: {responseSendConfirmation.StatusCode}");

            switch (responseSendConfirmation.StatusCode)
            {
                case HttpStatusCode.OK:
                    return await WaitConfirmationSecondPlayer();

                case HttpStatusCode.Conflict:
                    var errorMsg = await responseSendConfirmation.Content.ReadAsStringAsync();
                    ConsoleHelper.WriteInConsole(errorMsg, ConsoleColor.Red);
                    _ = Console.ReadLine();
                    _ = await _gameService.ExitFromRoomAsync();
                    return false;

                default:
                    break;
            }

            return true;
        }

        public async Task ShowEnemyBarAsync()
        {
            var responsePlayerMessage = await _gameService.GetResultsAsync();
            LogInformationAboutClass(nameof(ShowEnemyBarAsync),
                $"Response: {responsePlayerMessage.StatusCode}");

            if (responsePlayerMessage.StatusCode == HttpStatusCode.OK)
            {
                var results = await responsePlayerMessage.Content.ReadAsAsync<ResultsDto>();
                var enemyBarTable = new List<ResultsDto>() { results }.ToStringTable(new[] {
                $"{results.LoginFirstPlayer}", " VS ", $"{results.LoginSecondPlayer}" },
                res => res.WinFirst,
                _ => " VS ",
                res => res.WinSecond);
                ConsoleHelper.WriteInConsole(enemyBarTable + "\n", ConsoleColor.Blue);
            }
        }

        public async Task<bool> WaitConfirmationSecondPlayer()
        {
            while (true)
            {
                Console.Clear();
                ConsoleHelper.WriteInConsole(new[] { "Waiting for enemy confirmation" },
                    ConsoleColor.Green, "");
                var responseConfirmation = await _gameService.CheckConfirmationAsync();
                LogInformationAboutClass(nameof(WaitConfirmationSecondPlayer),
                    $"Response: {responseConfirmation.StatusCode}");

                switch (responseConfirmation.StatusCode)
                {
                    case HttpStatusCode.OK:
                        await _gameState.InvokeMenuAsync();
                        return true;

                    case HttpStatusCode.NotFound:
                        var time = await responseConfirmation.Content.ReadAsStringAsync();
                        ConsoleHelper.WriteInConsole(new[] { $"Time: {time}" }, ConsoleColor.Red, "");
                        Thread.Sleep(1000);
                        break;

                    case HttpStatusCode.Conflict:
                        var errorMsg = await responseConfirmation.Content.ReadAsStringAsync();
                        ConsoleHelper.WriteInConsole(errorMsg, ConsoleColor.Red);
                        _ = Console.ReadLine();
                        _ = _gameService.ExitFromRoomAsync();
                        return false;

                    default:
                        break;
                }
            }
        }

        public async Task ExitAsync()
        {
            LogInformationAboutClass(nameof(ExitAsync), "Player exit from room.");
            _ = await _gameService.ExitFromRoomAsync();
        }
    }
}
