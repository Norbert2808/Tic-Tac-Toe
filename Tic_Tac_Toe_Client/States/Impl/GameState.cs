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
                await ShowEnemyBar();
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
                    ConsoleHelper.WriteInConsole(new[]
                        {
                            "1 -- Do move",
                            "2 -- Surender",
                            "0 -- Exit"
                        },
                        ConsoleColor.Cyan);

                    try
                    {
                        ConsoleHelper.ReadIntFromConsole(out var choose);
                        switch (choose)
                        {
                            case 1:
                                await MakeMoveAsync();
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
                        ConsoleHelper.WriteInConsole(new[] { "Failed to connect with server!"},
                            ConsoleColor.Red);
                        _ = Console.ReadLine();
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
        
        public async Task MakeMoveAsync()
        {
            while (true)
            {
                Console.Clear();
                await ShowEnemyBar();
                _board.Draw();

                ConsoleHelper.WriteInConsole(new[] { "Input number of cell[1;9]:" }, ConsoleColor.Green, "");
                ConsoleHelper.ReadIntFromConsole(out var index);
                index--;
                ConsoleHelper.WriteInConsole(new[] { "Input number[1;9]:" }, ConsoleColor.Green, "");
                ConsoleHelper.ReadIntFromConsole(out var number);

                var response = await _gameService.MakeMoveAsync(index, number);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _board.SetNumberByIndex(new MoveDto(index, number), _iAmFirst);
                    break;
                }

                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    _board.SetNumberByIndex(new MoveDto(index, number), _iAmFirst);
                    _isEndOfGame = true;
                    _winnerFirst = _iAmFirst;
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

        public async Task ShowEnemyBar()
        {
            var responsePlayerMessage = await _gameService.CheckRoomAsync();

            if (responsePlayerMessage.StatusCode == HttpStatusCode.OK)
            {
                var opponents = await responsePlayerMessage.Content.ReadAsAsync<string[]>();
                ConsoleHelper.WriteInConsole($"{opponents[0]} -- VS -- {opponents[1]}\n", ConsoleColor.Cyan);
                var color = _iAmFirst ? ConsoleColor.Green : ConsoleColor.Red;
                ConsoleHelper.WriteInConsole($"Your color is {color}\n", color);
            }
        }

        public async Task ExitFromRoomAsync()
        {
            _ = await _gameService.ExitFromRoomAsync();
        }
    }
}
