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
            _logger.LogInformation("Invoke Round state class ");

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
                    _logger.LogError(ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "It's not a number!" },
                        ConsoleColor.Red);
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

        public async Task<bool> WaitingStartGame()
        {
            Console.Clear();
            var responseSendConfirmation = await _gameService.SendConfirmationAsync();

            switch (responseSendConfirmation.StatusCode)
            {
                case HttpStatusCode.OK:
                    _logger.LogInformation("RoundMenuState::WaitingStartGame::Successful response 200");
                    return await WaitConfirmationSecondPlayer();

                case HttpStatusCode.Conflict:
                    _logger.LogInformation("RoundMenuState::WaitingStartGame::Response: Conflict 409");
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

            if (responsePlayerMessage.StatusCode == HttpStatusCode.OK)
            {
                _logger.LogInformation("RoundMenuState::ShowEnemyBarAsync::Successful response 200");
                var results = await responsePlayerMessage.Content.ReadAsAsync<ResultsDto>();
                var enemyBarTable = new List<ResultsDto>() { results }.ToStringTable(new[] {
                $"{results.LoginFirstPlayer}", " VS ", $"{results.LoginSecondPlayer}" },
                res => res.WinFirst,
                res => " VS ",
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

                if (responseConfirmation.StatusCode == HttpStatusCode.OK)
                {
                    _logger.LogInformation("RoundMenuState::WaitConfirmationSecondPlayer::Successful response 200");
                    await _gameState.InvokeMenuAsync();
                    return true;
                }

                if (responseConfirmation.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("RoundMenuState::WaitConfirmationSecondPlayer::Response: NotFound 404");
                    var time = await responseConfirmation.Content.ReadAsStringAsync();
                    ConsoleHelper.WriteInConsole(new[] { $"Time: {time}" }, ConsoleColor.Red, "");
                    Thread.Sleep(1000);
                }

                if (responseConfirmation.StatusCode == HttpStatusCode.Conflict)
                {
                    _logger.LogInformation("RoundMenuState::WaitConfirmationSecondPlayer::Response: Conflict 409");

                    var errorMsg = await responseConfirmation.Content.ReadAsStringAsync();
                    ConsoleHelper.WriteInConsole(errorMsg, ConsoleColor.Red);
                    _ = Console.ReadLine();
                    _ = _gameService.ExitFromRoomAsync();
                    return false;
                }
            }
        }

        public async Task ExitAsync()
        {
            _ = await _gameService.ExitFromRoomAsync();
        }
    }
}
