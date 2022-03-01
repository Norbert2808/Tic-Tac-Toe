using System.Net;
using Microsoft.Extensions.Logging;
using TicTacToe.Client.Enums;
using TicTacToe.Client.Services;

namespace TicTacToe.Client.States.Impl;

public class GameMenuState : IGameMenuState
{
    private readonly IGameService _gameService;

    private readonly IGameState _gameState;

    private readonly ILogger<IGameMenuState> _logger;

    public GameMenuState(IGameState gameState,
        IGameService gameService,
        ILogger<IGameMenuState> logger)
    {
        _gameService = gameService;
        _gameState = gameState;
        _logger = logger;
    }

    public async Task InvokeMenuAsync()
    {
        _logger.LogInformation("Class MainMenuState. InvokeAsync method");
        
        while (true)
        {
            Console.Clear();
            ConsoleHelper.WriteInConsole(new []
            {
                "Game Menu",
                "Please choose room:",
                "1 -- Create private room",
                "2 -- Connect to private room",
                "3 -- Public room",
                "4 -- Practice room",
                "0 -- Close"
            }, ConsoleColor.Cyan);

            RoomType type = default;
            var roomId = "";
            var isConnecting = false;
            try
            {
                ConsoleHelper.ReadIntFromConsole(out var choose);
                switch (choose)
                {
                    case 1:
                        _logger.LogInformation("Creating private room.");
                        type = RoomType.Private;
                        break;
                    
                    case 2:
                        _logger.LogInformation("Connect to private room executed");
                        type = RoomType.Private;
                        ConsoleHelper.WriteInConsole(new []{ "Please enter token:"}, 
                            ConsoleColor.Yellow );
                        roomId = Console.ReadLine();
                        isConnecting = true;
                        break;
                        
                    case 3:
                        _logger.LogInformation("Creating public room.");
                        type = RoomType.Public;
                        break;

                    case 4:
                        _logger.LogInformation("Creating practice room.");
                        type = RoomType.Practice;
                        break;
                    case 0:
                        return;
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

            await StartConnectionWithRoomAsync(type, roomId!, isConnecting);
        }
    }

    public async Task StartConnectionWithRoomAsync(RoomType type, string roomId, bool isConnecting)
    {
        _logger.LogInformation("Creating room.");

        var response = await _gameService.StartRoomAsync(type, roomId, isConnecting);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (type == RoomType.Public)
            {
                ConsoleHelper.WriteInConsole(new []{ "Room was found! Please, be wait when your" + 
                                                     " opponent will entering." }, ConsoleColor.Green, "");
            }

            if (type == RoomType.Private)
            {
                ConsoleHelper.WriteInConsole(new []{ "Your private token:" + 
                                                     $"{ await response.Content.ReadAsStringAsync()}", 
                        "Please, be wait when your opponent will entering." },
                    ConsoleColor.Green, "");
            }

            await WaitSecondPlayerAsync();
        }   
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var errorMessage = await GetMessageFromResponseAsync(response);
            ConsoleHelper.WriteInConsole(new []{ errorMessage }, ConsoleColor.Red);
            Console.ReadLine();
        }
    }

    public async Task WaitSecondPlayerAsync()
    {
        _logger.LogInformation("Waiting second player.");
        while (true)
        {
            var response = await _gameService.CheckSecondPlayerAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                await _gameState.InvokeMenuAsync();
                return;
            }
        }
    }
    
    public async Task<string> GetMessageFromResponseAsync(HttpResponseMessage response)
    {
        return await response.Content.ReadAsStringAsync();
    }
}
