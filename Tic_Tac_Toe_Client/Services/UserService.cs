using System.Net.Http.Formatting;
using Tic_Tac_Toe.Client.Models;

namespace Tic_Tac_Toe.Client.Services
{
    internal class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> LoginAsync(string login, string password)
        {
            var user = new User() { Login = login, Password = password };
            var response = await _httpClient.PostAsync("api/Account/login", user, new JsonMediaTypeFormatter());

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _httpClient.DefaultRequestHeaders.Clear();

            }

            return response;
        }

        public async Task<HttpResponseMessage> RegistrationAsync(string login, string password)
        {
            var user = new User() { Login = login, Password = password };
            var response = await _httpClient.PostAsync("api/Account/registration", user, new JsonMediaTypeFormatter());

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _httpClient.DefaultRequestHeaders.Clear();
            }

            return response;
        }

    }
}
