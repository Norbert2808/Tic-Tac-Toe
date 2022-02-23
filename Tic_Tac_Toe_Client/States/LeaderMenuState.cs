using Microsoft.Extensions.Logging;
using Tic_Tac_Toe.Client.Services;

namespace Tic_Tac_Toe.Client.States;

public class LeaderMenuState : IState
{
    private readonly IStatisticService _statisticService;
    
    private readonly ILogger<LeaderMenuState> _logger;
    
    public LeaderMenuState(IStatisticService statisticService, ILogger<LeaderMenuState> logger)
    {
        _statisticService = statisticService;
        _logger = logger;
    }
    
    public async Task InvokeAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("LeaderBoard menu");
            Console.WriteLine("1 -- By wins");
            Console.WriteLine("2 -- By times");
            Console.WriteLine("3 -- Close");
            
            try
            {
                ConsoleHelper.ReadIntFromConsole(out var choose);
                switch (choose)
                {
                    case 1:
                        break;
                        
                    case 2:
                        break;
                    
                    case 3:
                        return;
                }
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
