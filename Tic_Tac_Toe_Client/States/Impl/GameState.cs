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
                await EnemyBarMenu();
                ConsoleHelper.WriteInConsole(new []
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
                        await ExitAsync();
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
                Console.ReadLine();
            }

        }
    }

    public async Task WaitingStartGame()
    {
        Console.Clear();
        var responseMessage = await _gameService.SendConfirmationAsync();

        if (responseMessage.StatusCode == HttpStatusCode.OK)
        {
            while (true)
            {
                Console.Clear();
                ConsoleHelper.WriteInConsole(new []{ "Waiting for enemy confirmation" },
                    ConsoleColor.Green, "");
                var responseConfirmation = await _gameService.CheckConfirmationAsync();

                if (responseConfirmation.StatusCode == HttpStatusCode.OK) 
                {
                    //ToDo Create Round
                    await GameMenu();
                    return;
                }

                if (responseConfirmation.StatusCode == HttpStatusCode.NotFound)
                {
                    var time = await responseConfirmation.Content.ReadAsStringAsync();
                    ConsoleHelper.WriteInConsole(new []{ $"Time: {time}" }, ConsoleColor.Red, "");
                    Thread.Sleep(1000);
                }

                if (responseConfirmation.StatusCode == HttpStatusCode.Conflict)
                {
                    await _gameService.ExitFromRoomAsync();
                    return;
                }
            }
        }
            
    }

    public async Task GameMenu()
    {
        while (true)
        {
            Console.Clear();
            await EnemyBarMenu();
            _board.Draw();
            ConsoleHelper.WriteInConsole(new []
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
                Console.ReadLine();
            }
        }
    }
    
    public async Task EnemyBarMenu()
    {
        var responsePlayerMessage = await _gameService.CheckRoomAsync();

        if (responsePlayerMessage.StatusCode == HttpStatusCode.OK)
        {
            var opponents = await  responsePlayerMessage.Content.ReadAsAsync<string[]>();
            Console.WriteLine($"{opponents[0]} -- VS -- {opponents[1]}");
        }
    }

    public async Task MakeMoveAsync()
    {
        while (true)
        {   
            Console.Clear();
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

    public async Task ExitAsync()
    {
        await _gameService.ExitFromRoomAsync();
    }
    
}
