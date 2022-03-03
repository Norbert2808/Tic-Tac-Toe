using System.Net;
using Microsoft.Extensions.Logging;
using TicTacToe.Client.Enums;
using TicTacToe.Client.Services;

namespace TicTacToe.Client.States.Impl;

public class RoomMenuState : IRoomMenuState
{
    private readonly IGameService _gameService;

    private readonly IRoundState _roundState;

    private readonly ILogger<IRoomMenuState> _logger;

    public RoomMenuState(IRoundState roundState,
        IGameService gameService,
        ILogger<IRoomMenuState> logger)
    {
        _gameService = gameService;
        _roundState = roundState;
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
                "Room Menu",
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
                            ConsoleColor.Yellow, "");
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

            await StartConnectionWithRoomAsync(type, roomId!, isConnecting);
        }
    }

    public async Task StartConnectionWithRoomAsync(RoomType type, string roomId, bool isConnecting)
    {
        _logger.LogInformation("Creating room.");

        var response = await _gameService.StartRoomAsync(type, roomId, isConnecting);

        var message = Array.Empty<string>();
        
        if (response.StatusCode == HttpStatusCode.OK)
        {
            if (type == RoomType.Public)
            {
                message = new[]
                {
                    "Room was found! Please, be wait when your" +
                    " opponent will entering."
                };
            }

            if (type == RoomType.Private)
            {
                message = new[]
                {
                    "Your private token:" +
                    $"{await response.Content.ReadAsStringAsync()}",
                    "Please, be wait when your opponent will entering."
                };
            }

            if (type == RoomType.Practice)
            {
                
            }
            
            await WaitSecondPlayerAsync(message);
        }   
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var errorMessage = await GetMessageFromResponseAsync(response);
            ConsoleHelper.WriteInConsole(new []{ errorMessage }, ConsoleColor.Red);
            Console.ReadLine();
        }
    }

    public async Task WaitSecondPlayerAsync(string[] message)
    {
        _logger.LogInformation("Waiting second player.");
        while (true)
        {
            Console.Clear();
            ConsoleHelper.WriteInConsole(message,
                ConsoleColor.Green, "");
            var response = await _gameService.CheckRoomAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                await _roundState.InvokeMenuAsync();
                return;
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var time = await response.Content.ReadAsStringAsync();
                ConsoleHelper.WriteInConsole(new []{ $"Time: {time}" }, ConsoleColor.Red, "");
                Thread.Sleep(5000);
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                await  _gameService.ExitFromRoomAsync();
                return;
            }
        }
    }
    
    public async Task<string> GetMessageFromResponseAsync(HttpResponseMessage response)
    {
        return await response.Content.ReadAsStringAsync();
    }
}
