using System.Net;
using Microsoft.Extensions.Logging;
using TicTacToe.Client.DTO;
using TicTacToe.Client.Models;
using TicTacToe.Client.Services;
using TicTacToe.Client.Tools;

namespace TicTacToe.Client.States.Impl
{
    public class GameState : IGameState
    {
        private readonly IGameService _gameService;

        private readonly ILogger<GameState> _logger;

        private readonly Board _board;

        private bool _isFirst;

        private bool _isEndOfGame;

        private bool _isActivePlayer;

        public GameState(IGameService gameService,
            ILogger<GameState> logger)
        {
            _gameService = gameService;
            _logger = logger;
            _board = new Board();
        }

        public async Task InvokeMenuAsync()
        {
            _logger.LogInformation("Invoke Game state class");

            await CheckRoundStateAsync();
            while (!_isEndOfGame)
            {
                Console.Clear();
                _board.Draw();

                _logger.LogInformation("Invoke game menu.");

                if (_isActivePlayer == _isFirst)
                {
                    _logger.LogInformation("Player waiting opponent's move.");
                    ConsoleHelper.WriteInConsole(new[] { "Please, Wait till the other player moves" },
                        ConsoleColor.Green, "");
                    await WaitMoveOpponentAsync();
                    continue;
                }
                else
                {
                    var color = _isFirst ? ConsoleColor.Green : ConsoleColor.Red;
                    ConsoleHelper.WriteInConsole($"Your color is {color}\n", color);
                    ConsoleHelper.WriteInConsole(new[]
                    {
                        "You have 20 seconds to move",
                        "1 -- Do move",
                        "2 -- Surrender",
                    },
                        ConsoleColor.Cyan);
                    try
                    {
                        ConsoleHelper.ReadIntFromConsole(out var choose);
                        switch (choose)
                        {
                            case 1:
                                var validMove = await MakeMoveAsync();
                                if (!validMove)
                                    continue;
                                break;

                            case 2:
                                await SurrenderAsync();
                                break;

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
                        continue;
                    }
                    catch (HttpRequestException ex)
                    {
                        _logger.LogError(ex.Message);
                        ConsoleHelper.WriteInConsole(new[] { "Failed to connect with server!" },
                                ConsoleColor.Red);
                        _ = Console.ReadLine();
                        continue;
                    }
                }
            }

            ResultMenu();
        }

        private void ResultMenu()
        {
            Console.Clear();
            _board.Draw();
            if (_isFirst == _isActivePlayer)
                DrawWin();
            else
                DrawLose();

            _ = Console.ReadLine();
        }

        private async Task CheckRoundStateAsync()
        {
            var response = await _gameService.CheckRoundStateAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var roundState = await response.Content.ReadAsAsync<RoundStateDto>();
                _board.SetBoard(roundState.Board);
                _isEndOfGame = roundState.IsFinished;
                _isFirst = roundState.IsFirstPlayer;
                _isActivePlayer = roundState.IsActiveFirstPlayer;
            }
        }

        public async Task<bool> MakeMoveAsync()
        {
            _logger.LogInformation("Player make a move");

            while (true)
            {
                var move = GetMoveFromPlayer();
                var response = await _gameService.MakeMoveAsync(move);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    await CheckRoundStateAsync();
                    return true;
                }

                var errorMsg = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    ConsoleHelper.WriteInConsole(new[] { errorMsg }, ConsoleColor.DarkRed);
                    _ = Console.ReadLine();
                    return false;
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    ConsoleHelper.WriteInConsole(new[] { errorMsg }, ConsoleColor.Red);
                    _isEndOfGame = true;
                    _ = Console.ReadLine();
                    return false;
                }

                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    _isEndOfGame = true;
                    ConsoleHelper.WriteInConsole(new[] { errorMsg + "\n" }, ConsoleColor.DarkRed);
                    _ = Console.ReadLine();
                    return false;
                }
            }
        }

        public async Task WaitMoveOpponentAsync()
        {
            _logger.LogInformation("Waiting opponent's move.");

            while (true)
            {
                var responseMessage = await _gameService.CheckMoveAsync();
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var roundState = await responseMessage.Content.ReadAsAsync<RoundStateDto>();
                    _board.SetBoard(roundState.Board);
                    _isFirst = roundState.IsFirstPlayer;
                    _isEndOfGame = roundState.IsFinished;
                    _isActivePlayer = roundState.IsActiveFirstPlayer;
                    break;
                }

                if (responseMessage.StatusCode == HttpStatusCode.Conflict)
                {
                    _isEndOfGame = true;
                    var errorMsg = await responseMessage.Content.ReadAsStringAsync();
                    ConsoleHelper.WriteInConsole(errorMsg + "\n", ConsoleColor.DarkRed);
                    _ = Console.ReadLine();
                    break;
                }
            }
        }

        private MoveDto GetMoveFromPlayer()
        {
            _logger.LogInformation("Get player's move.");

            while (true)
            {
                Console.Clear();
                _board.Draw();

                int index;
                int number;
                try
                {
                    ConsoleHelper.WriteInConsole(new[] { "Input number of cell[1;9]:" }, ConsoleColor.Green, "");
                    ConsoleHelper.ReadIntFromConsole(out index);
                    ConsoleHelper.WriteInConsole(new[] { "Input number[1;9]:" }, ConsoleColor.Green, "");
                    ConsoleHelper.ReadIntFromConsole(out number);
                    index -= 1;
                }
                catch (FormatException ex)
                {
                    _logger.LogError(ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "It's not a number!" }, ConsoleColor.DarkRed);
                    _ = Console.ReadLine();
                    continue;
                }

                return new MoveDto(index, number);
            }
        }

        private void DrawWin()
        {
            ConsoleHelper.WriteInConsole(new[]
            {
                "╔╗╔╗╔══╗╔╗╔╗───╔╗╔╗╔╗╔══╗╔╗─╔╗",
                "║║║║║╔╗║║║║║───║║║║║║╚╗╔╝║╚═╝║",
                "║╚╝║║║║║║║║║───║║║║║║─║║─║╔╗─║",
                "╚═╗║║║║║║║║║───║║║║║║─║║─║║╚╗║",
                "─╔╝║║╚╝║║╚╝║───║╚╝╚╝║╔╝╚╗║║─║║",
                "─╚═╝╚══╝╚══╝───╚═╝╚═╝╚══╝╚╝─╚╝"
            }, ConsoleColor.Green);
        }

        private void DrawLose()
        {
            ConsoleHelper.WriteInConsole(new[]
                        {
                "╔╗╔╗╔══╗╔╗╔╗───╔╗──╔══╗╔══╗╔═══╗",
                "║║║║║╔╗║║║║║───║║──║╔╗║║╔═╝║╔══╝",
                "║╚╝║║║║║║║║║───║║──║║║║║╚═╗║╚══╗",
                "╚═╗║║║║║║║║║───║║──║║║║╚═╗║║╔══╝",
                "─╔╝║║╚╝║║╚╝║───║╚═╗║╚╝║╔═╝║║╚══╗",
                "─╚═╝╚══╝╚══╝───╚══╝╚══╝╚══╝╚═══╝"
            }, ConsoleColor.Red);
        }

        public async Task SurrenderAsync()
        {
            var responseSurrender = await _gameService.SurrenderAsync();
            if (responseSurrender.StatusCode == HttpStatusCode.OK)
            {
                await CheckRoundStateAsync();
            }
        }
    }
}
