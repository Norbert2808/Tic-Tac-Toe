using System.Net;
using Microsoft.Extensions.Logging;
using TicTacToe.Client.DTO;
using TicTacToe.Client.Models;
using TicTacToe.Client.Services;

namespace TicTacToe.Client.States.Impl
{
    public class GameState : IGameState
    {
        private readonly IGameService _gameService;

        private readonly ILogger<GameState> _logger;

        private readonly Board _board;

        private bool _iAmFirst;

        private bool _isEndOfGame;

        private bool _winnerFirst;

        public GameState(IGameService gameService,
            ILogger<GameState> logger)
        {
            _gameService = gameService;
            _logger = logger;
            _board = new Board();
            _isEndOfGame = false;
        }

        public async Task InvokeMenuAsync()
        {
            _isEndOfGame = false;
            _board.SetDefaultValuesInBoard();
            _iAmFirst = await _gameService.CheckPlayerPosition();
            var myTurnToMove = _iAmFirst;

            while (true)
            {
                Console.Clear();
                _board.Draw();

                if (_isEndOfGame)
                {
                    var message = _iAmFirst == _winnerFirst ? "YOU WIN!" : "YOU LOST!";
                    ConsoleHelper.WriteInConsole(new string[] { message }, ConsoleColor.Magenta);
                    _ = Console.ReadLine();
                    break;
                }

                if (myTurnToMove)
                {
                    var color = _iAmFirst ? ConsoleColor.Green : ConsoleColor.Red;
                    ConsoleHelper.WriteInConsole($"Your color is {color}\n", color);
                    ConsoleHelper.WriteInConsole(new[]
                        {
                            "1 -- Do move",
                            "2 -- Surender",
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
                                break;

                            default:
                                continue;
                        }
                    }
                    catch (FormatException ex)
                    {
                        _logger.LogError(ex.Message);
                        ConsoleHelper.WriteInConsole(new[] { "It's not a number!" }, ConsoleColor.DarkRed);
                        _ = Console.ReadLine();
                        continue;
                    }
                    catch (HttpRequestException ex)
                    {
                        _logger.LogError(ex.Message);
                        ConsoleHelper.WriteInConsole(new[] { "Failed to connect with server!" },
                            ConsoleColor.DarkRed);
                        _ = Console.ReadLine();
                        continue;
                    }

                    myTurnToMove = false;
                }
                else
                {
                    ConsoleHelper.WriteInConsole(new[] { "Please, Wait till the other player moves" },
                        ConsoleColor.Green, "");
                    await WaitMoveOpponentAsync();

                    myTurnToMove = true;
                }
            }
        }

        public async Task<bool> MakeMoveAsync()
        {
            while (true)
            {
                var move = GetMoveFromPlayer();
                var response = await _gameService.MakeMoveAsync(move);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _board.SetNumberByIndex(move, _iAmFirst);
                    return true;
                }

                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    _board.SetNumberByIndex(move, _iAmFirst);
                    _isEndOfGame = true;
                    _winnerFirst = _iAmFirst;
                    return true;
                }

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var errorMes = await response.Content.ReadAsStringAsync();
                    ConsoleHelper.WriteInConsole(new string[] { errorMes }, ConsoleColor.DarkRed);
                    _ = Console.ReadLine();
                    return false;
                }
            }
        }

        public async Task WaitMoveOpponentAsync()
        {
            while (true)
            {
                var responseMessage = await _gameService.CheckMoveAsync();
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var move = await responseMessage.Content.ReadAsAsync<MoveDto>();
                    _board.SetNumberByIndex(move, !_iAmFirst);
                    break;
                }
                if (responseMessage.StatusCode == HttpStatusCode.Accepted)
                {
                    var move = await responseMessage.Content.ReadAsAsync<MoveDto>();
                    _board.SetNumberByIndex(move, !_iAmFirst);
                    _isEndOfGame = true;
                    _winnerFirst = !_iAmFirst;
                    break;
                }
            }
        }

        private MoveDto GetMoveFromPlayer()
        {
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

        public async Task ExitFromRoomAsync()
        {
            _ = await _gameService.ExitFromRoomAsync();
        }
    }
}
