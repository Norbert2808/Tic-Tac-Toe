using System.Net.Http.Formatting;
using Tic_Tac_Toe.Client.Models;

namespace Tic_Tac_Toe.Client.Services.Impl
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
                var registerLogin = await response.Content.ReadAsStringAsync();
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Login", registerLogin);
            }

            return response;
        }

        public async Task<HttpResponseMessage> RegistrationAsync(string login, string password)
        {
            var user = new User() { Login = login, Password = password };
            var response = await _httpClient.PostAsync("api/Account/registration", user, new JsonMediaTypeFormatter());

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var registerLogin = await response.Content.ReadAsStringAsync();
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Login", registerLogin);
            }

            return response;
        }

    }
}
