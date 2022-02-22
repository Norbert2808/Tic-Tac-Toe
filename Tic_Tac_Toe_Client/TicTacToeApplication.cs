using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Tic_Tac_Toe.Client.Models;
using Tic_Tac_Toe.Client.Services;
using Tic_Tac_Toe.Client.States;

namespace Tic_Tac_Toe.Client
{
    internal class TicTacToeApplication
    {
        public static async Task<int> Main()
        {
            var provider = GetServiceProvider();

            while (true)
            {
                var authorizationMenu = provider.GetRequiredService<AuthorizationMenuState>();
                await authorizationMenu.InvokeAsync();
                return 0;
            }
        }

        private static IServiceProvider GetServiceProvider()
        {
            var httpClient = ConfigureHttpClient();
            var serviceCollection = new ServiceCollection();

            _ = serviceCollection.AddSingleton(provider => new UserService(httpClient));

            _ = serviceCollection
                .AddTransient<AuthorizationMenuState>()
                .AddTransient<IState, MainMenuState>();


            return serviceCollection.BuildServiceProvider();
        }

        private static HttpClient ConfigureHttpClient()
        {
            var client = new HttpClient();
            var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "userConfig.json");
            var json = File.ReadAllText(path);
            var option = JsonSerializer.Deserialize<ClientOption>(json);
            client.BaseAddress = new Uri(option.UriAddress);
            return client;
        }
    }
}
