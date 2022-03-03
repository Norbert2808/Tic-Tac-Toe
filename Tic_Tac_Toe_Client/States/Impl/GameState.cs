using System.Net;
using Microsoft.Extensions.Logging;
using TicTacToe.Client.Models;
using TicTacToe.Client.Services;

namespace TicTacToe.Client.States.Impl
{
    public class GameState : IGameState
    {

        private readonly IGameService _gameService;

        private readonly ILogger<GameState> _logger;

        private readonly Board _board;

        public GameState(IGameService gameService,
            ILogger<GameState> logger)
        {
            _gameService = gameService;
            _logger = logger;
            _board = new Board();
        }

        public async Task InvokeMenuAsync()
        {
            while (true)
            {
                Console.Clear();
                await ShowEnemyBar();
                _board.Draw();
                ConsoleHelper.WriteInConsole(new[]
                {
                "1 -- Do move",
                "2 -- Surender",
                "0 -- Exit"
            }, ConsoleColor.Cyan);

                try
                {
                    ConsoleHelper.ReadIntFromConsole(out var choose);
                    switch (choose)
                    {
                        case 1:
                            break;

                        case 2:
                            break;

                        case 0:
                            continue;

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

        public async Task MakeMoveAsync()
        {
            while (true)
            {
                Console.Clear();
                _board.Draw();
                ConsoleHelper.WriteInConsole(new[] { "Input index of сell:" }, ConsoleColor.Green, "");
                ConsoleHelper.ReadIntFromConsole(out var index);
                ConsoleHelper.WriteInConsole(new[] { "Input number:" }, ConsoleColor.Green, "");
                ConsoleHelper.ReadIntFromConsole(out var number);

                var response = await _gameService.MakeMoveAsync(index, number);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    break;
                }

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {

                }
            }

            //Тут обновить борду. И ждать ход противника.

        }

        public async Task WaitMoveOpponentAsync()
        {
            while (true)
            {
                var responseMessage = await _gameService.CheckMoveAsync();
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    /// _board.SetNumberByIndex();
                }
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
    }
}
