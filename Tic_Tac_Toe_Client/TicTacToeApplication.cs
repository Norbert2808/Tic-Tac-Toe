using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using TicTacToe.Client.Models;
using TicTacToe.Client.Services;
using TicTacToe.Client.Services.Impl;
using TicTacToe.Client.States;
using TicTacToe.Client.States.Impl;

namespace TicTacToe.Client
{
    public static class TicTacToeApplication
    {
        public static async Task Run()
        {
            var provider = GetServiceProvider();

            var authorizationMenu = provider.GetRequiredService<AuthorizationMenuState>();
            await authorizationMenu.InvokeMenuAsync();
        }

        private static IServiceProvider GetServiceProvider()
        {
            var httpClient = ConfigureHttpClient();
            var serviceCollection = new ServiceCollection();

            _ = serviceCollection
                .AddSingleton<IUserService>(_ => new UserService(httpClient))
                .AddSingleton<IStatisticService>(_ => new StatisticService(httpClient))
                .AddSingleton<IGameService>(_ => new GameService(httpClient))
                .AddSingleton<ISettingsService, SettingsService>();

            _ = serviceCollection
                .AddTransient<AuthorizationMenuState>()
                .AddTransient<IMainMenuState, MainMenuState>()
                .AddTransient<ISettingsState, SettingsState>()
                .AddTransient<ILeaderMenuState, LeaderMenuState>()
                .AddTransient<IRoomMenuState, RoomMenuState>()
                .AddTransient<IRoundMenuState, RoundMenuState>()
                .AddTransient<IGameState, GameState>()
                .AddTransient<IPrivateStatisticState, PrivateStatisticState>();


            var serilog = new LoggerConfiguration()
                .WriteTo
                .File("clientLoggs.log")
                .CreateLogger();
            _ = serviceCollection.AddLogging(builder => builder
                .ClearProviders()
                .AddSerilog(serilog, true));

            return serviceCollection.BuildServiceProvider();
        }

        private static HttpClient ConfigureHttpClient()
        {
            var client = new HttpClient();
            if (!File.Exists("userConfig.json"))
                return client;

            var json = File.ReadAllText("userConfig.json");
            var option = JsonSerializer.Deserialize<ClientOption>(json);

            if (option is null || option.UriAddress is null)
                return client;

            client.BaseAddress = new Uri(option.UriAddress);
            return client;
        }
    }
}
