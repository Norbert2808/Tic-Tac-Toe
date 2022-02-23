using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Tic_Tac_Toe.Client.Models;
using Tic_Tac_Toe.Client.Services;
using Tic_Tac_Toe.Client.Services.Impl;
using Tic_Tac_Toe.Client.States;

namespace Tic_Tac_Toe.Client
{
    internal class TicTacToeApplication
    {
        public static async Task Main()
        {
            var provider = GetServiceProvider();

            while (true)
            {
                var authorizationMenu = provider.GetRequiredService<AuthorizationMenuState>();
                await authorizationMenu.InvokeAsync();
                return;
            }
        }

        private static IServiceProvider GetServiceProvider()
        {
            var httpClient = ConfigureHttpClient();
            var serviceCollection = new ServiceCollection();
            
            _ = serviceCollection.AddSingleton<IUserService>(_ => new UserService(httpClient));
            _ = serviceCollection.AddSingleton<IStatisticService>(_ => new StatisticService(httpClient));
            _ = serviceCollection.AddSingleton<IGameService>(_ => new GameService(httpClient));

            _ = serviceCollection
                .AddTransient<AuthorizationMenuState>()
                .AddTransient<MainMenuState>()
                .AddTransient<LeaderMenuState>()
                .AddTransient<GameMenuState>();

            var serilog = new LoggerConfiguration()
                .WriteTo
                .File("clientLoggs.log")
                .CreateLogger();
            _ = serviceCollection.AddLogging(builder => builder.AddSerilog(serilog, true));
            
            return serviceCollection.BuildServiceProvider();
        }

        private static HttpClient ConfigureHttpClient()
        {
            var client = new HttpClient();
            var json = File.ReadAllText("userConfig.json");
            var option = JsonSerializer.Deserialize<ClientOption>(json);
            client.BaseAddress = new Uri(option.UriAddress);
            return client;
        }
    }
}
