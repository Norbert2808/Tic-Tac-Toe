using Microsoft.Extensions.Logging;
using Tic_Tac_Toe.Client.Enums;
using Tic_Tac_Toe.Client.Services;
using Tic_Tac_Toe.Client.Services.Impl;

namespace Tic_Tac_Toe.Client.States;

public class GameMenuState : IState
{
    private readonly IGameService _gameService;
    
    private readonly ILogger<GameMenuState> _logger;

    public GameMenuState(IGameService gameService,
        ILogger<GameMenuState> logger)
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
            Console.WriteLine("Other -- Close");

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
                        ConsoleHelper.WriteInConsole(new []{ "Please write custom room id" },
                            ConsoleColor.Yellow);
                        ConsoleHelper.ReadStringFromConsole(out roomId);
                        break;
                    
                    case 2:
                        _logger.LogInformation("Creating public room.");
                        type = RoomType.Public;
                        break;

                    case 3:
                        _logger.LogInformation("Creating practice room.");
                        type = RoomType.Practice;
                        break;
                    default:
                        return;
                }
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex.Message);
            }

            await StartGameAsync(type, roomId);
        }
    }
    
    private async Task StartGameAsync(RoomType type, string roomId)
    {
        _logger.LogInformation("Game start");

        var response = await _gameService.StartSessionAsync(type, roomId);
        
        Console.WriteLine(await response.Content.ReadAsStringAsync());
        Console.ReadLine();
    }
}
