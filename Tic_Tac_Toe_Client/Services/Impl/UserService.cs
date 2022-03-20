using System.Net;
using System.Net.Http.Formatting;
using TicTacToe.Client.DTO;

namespace TicTacToe.Client.Services.Impl
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        private const string ControllerPath = "api/account/";

        public string? Login { get; set; }

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> LoginAsync(string login, string password)
        {
            var user = new UserDto { Login = login, Password = password };

            var response = await _httpClient.PostAsync(ControllerPath + "login",
                user,
                new JsonMediaTypeFormatter());

            if (response.StatusCode == HttpStatusCode.OK)
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

            var response = await _httpClient.PostAsync(ControllerPath + "registration",
                user,
                new JsonMediaTypeFormatter());

            if (response.StatusCode == HttpStatusCode.OK)
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
            var response = await _httpClient.DeleteAsync(ControllerPath + "logout");

            if (response.StatusCode == HttpStatusCode.OK)
                _httpClient.DefaultRequestHeaders.Clear();

            return response;
        }
    }
}
