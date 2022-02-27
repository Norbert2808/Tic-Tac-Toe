using System.Net;
using Microsoft.Extensions.Logging;
using TicTacToe.Client.Models;
using TicTacToe.Client.Services;

namespace TicTacToe.Client.States.Impl;

public class GameState : IGameState
{
    private readonly IGameService _gameService;

    private readonly Board _board;

    private readonly ILogger<GameState> _logger;

    public GameState(IGameService gameService, ILogger<GameState> logger)
    {
        _gameService = gameService;
        _board = new Board();
        _logger = logger;
    }
    
    public async Task InvokeMenuAsync()
    {
        _logger.LogInformation("Invoke Game state class ");

        while (true)
        {
            try
            {

                Console.Clear();
                _board.Draw();
                Console.WriteLine("1 -- To make a move");
                Console.WriteLine("0 -- Exit");
                ConsoleHelper.ReadIntFromConsole(out var choose);
                switch (choose)
                {
                    case 1:
                        await MakeMoveAsync();
                        break;
                    case 0:
                        return;
                }
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex.Message);
            }

        }
    }

    public async Task MakeMoveAsync()
    {
        while (true)
        {   Console.Clear();
            _board.Draw();
            ConsoleHelper.WriteInConsole(new []{ "Input index of сell:" }, ConsoleColor.Green, "");
            ConsoleHelper.ReadIntFromConsole(out var index);
            ConsoleHelper.WriteInConsole(new []{ "Input number:" }, ConsoleColor.Green, "");
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

}
