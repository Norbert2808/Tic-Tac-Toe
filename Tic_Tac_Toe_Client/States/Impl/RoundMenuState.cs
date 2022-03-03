using System.Net;
using Microsoft.Extensions.Logging;
using TicTacToe.Client.Services;

namespace TicTacToe.Client.States.Impl
{
    public class RoundMenuState : IRoundState
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
                    await ShowEnemyBar();
                    ConsoleHelper.WriteInConsole(new[]
                    {
                    "------------------",
                    "1 -- Start new round",
                    "0 -- Exit"
                }, ConsoleColor.Cyan);

                    ConsoleHelper.ReadIntFromConsole(out var choose);
                    switch (choose)
                    {
                        case 1:
                            await WaitingStartGame();
                            return;

                        case 0:
                            await ExitFromRoomAsync();
                            return;

                        default:
                            continue;
                    }
                }
                catch (FormatException ex)
                {
                    _logger.LogError(ex.Message);
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

        public async Task WaitingStartGame()
        {
            Console.Clear();
            var responseSendConfirmation = await _gameService.SendConfirmationAsync();

            if (responseSendConfirmation.StatusCode == HttpStatusCode.OK)
            {
                while (true)
                {
                    Console.Clear();
                    ConsoleHelper.WriteInConsole(new[] { "Waiting for enemy confirmation" },
                        ConsoleColor.Green, "");
                    var responseConfirmation = await _gameService.CheckConfirmationAsync();

                    if (responseConfirmation.StatusCode == HttpStatusCode.OK)
                    {
                        //ToDo Create Round
                        await _gameState.InvokeMenuAsync();
                        return;
                    }

                    if (responseConfirmation.StatusCode == HttpStatusCode.NotFound)
                    {
                        var time = await responseConfirmation.Content.ReadAsStringAsync();
                        ConsoleHelper.WriteInConsole(new[] { $"Time: {time}" }, ConsoleColor.Red, "");
                        Thread.Sleep(1000);
                    }

                    if (responseConfirmation.StatusCode == HttpStatusCode.Conflict)
                    {
                        var errorMsg = await GetMessageFromResponseAsync(responseConfirmation);
                        ConsoleHelper.WriteInConsole(new[] { errorMsg }, ConsoleColor.Red);
                        _ = Console.ReadLine();
                        _ = await _gameService.ExitFromRoomAsync();
                        return;
                    }
                }
            }

            if (responseSendConfirmation.StatusCode == HttpStatusCode.Conflict)
            {
                ConsoleHelper.WriteInConsole(new[] { "You didn`t confirm the game. Room was closed." }, ConsoleColor.Red);
                _ = Console.ReadLine();
                _ = await _gameService.ExitFromRoomAsync();
            }

        }

        public async Task ShowEnemyBar()
        {
            var responsePlayerMessage = await _gameService.CheckRoomAsync();

            if (responsePlayerMessage.StatusCode == HttpStatusCode.OK)
            {
                var opponents = await responsePlayerMessage.Content.ReadAsAsync<string[]>();
                Console.WriteLine($"{opponents[0]} -- VS -- {opponents[1]}");
            }
        }


        public async Task ExitFromRoomAsync()
        {
            _ = await _gameService.ExitFromRoomAsync();
        }

        public async Task<string> GetMessageFromResponseAsync(HttpResponseMessage response)
        {
            return await response.Content.ReadAsStringAsync();
        }

    }
}
