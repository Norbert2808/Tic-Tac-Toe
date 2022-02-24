using System.Net;
using Microsoft.Extensions.Logging;
using TicTacToe.Client.Enums;
using TicTacToe.Client.Services;

namespace TicTacToe.Client.States.Impl;

public class GameMenuState : IGameMenuState
{
    private readonly IGameService _gameService;

    private readonly ILogger<IGameMenuState> _logger;

    public GameMenuState(IGameService gameService,
        ILogger<IGameMenuState> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }

    public async Task InvokeAsync()
    {
        _logger.LogInformation("Class MainMenuState. InvokeAsync method");
        
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Game Menu");
            Console.WriteLine("Please choose room:");
            Console.WriteLine("1 -- Private room");
            Console.WriteLine("2 -- Public room");
            Console.WriteLine("3 -- Practice room");
            Console.WriteLine("0 -- Close");

            RoomType type = default;
            var roomId = "";
            try
            {
                ConsoleHelper.ReadIntFromConsole(out var choose);
                switch (choose)
                {
                    case 1:
                        _logger.LogInformation("Creating private room.");
                        type = RoomType.Private;
                        ConsoleHelper.WriteInConsole(new[] {"Please write custom room id"},
                            ConsoleColor.Yellow);
                        roomId = Console.ReadLine();
                        break;

                    case 2:
                        _logger.LogInformation("Creating public room.");
                        type = RoomType.Public;
                        break;

                    case 3:
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
            }

            await StartConnectionWithRoomAsync(type, roomId);
        }
    }

    public async Task StartConnectionWithRoomAsync(RoomType type, string roomId)
    {
        _logger.LogInformation("Creating room.");

        var response = await _gameService.StartSessionAsync(type, roomId);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            Console.WriteLine("Room was found! Please, be wait when your opponent will entering.");
            await WaitSecondPlayerAsync();
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
                Console.WriteLine("Player was found. Please, press to start.");
                Console.ReadLine();
                return;
            }
        }
    }
}
