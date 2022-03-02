using System.Net.Http.Formatting;
using TicTacToe.Client.DTO;

namespace TicTacToe.Client.Services.Impl
{
    internal class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        private string? Login { get; set; }

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> LoginAsync(string login, string password)
        {
            var user = new UserDto { Login = login, Password = password };
            
            var response = await _httpClient.PostAsync("api/Account/login",
                user,
                new JsonMediaTypeFormatter());

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var registerLogin = await response.Content.ReadAsStringAsync();
                Login = registerLogin;
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Login", registerLogin);
            }

            return response;
        }

        public async Task<HttpResponseMessage> RegistrationAsync(string login, string password)
        {
            var user = new UserDto { Login = login, Password = password };
            
            var response = await _httpClient.PostAsync("api/Account/registration",
                user,
                new JsonMediaTypeFormatter());

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var registerLogin = await response.Content.ReadAsStringAsync();
                Login = registerLogin;
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Login", registerLogin);
            }

            return response;
        }

        public async Task<HttpResponseMessage> LogoutAsync()
        {
            return await _httpClient.PostAsync("api/Account/logout", Login, new JsonMediaTypeFormatter());
        }
    }
}
