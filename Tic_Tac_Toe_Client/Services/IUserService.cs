namespace TicTacToe.Client.Services
{
    internal interface IUserService
    {
        /// <summary>
        /// Sends the request for log-in the game.
        /// </summary>
        /// <param name="login">User's login.</param>
        /// <param name="password">User's password.</param>
        /// <returns>
        /// The task result contains <see cref="HttpResponseMessage"/>.
        /// </returns>
        Task<HttpResponseMessage> LoginAsync(string login, string password);

        /// <summary>
        /// Sends the request for registration in the game.
        /// </summary>
        /// <param name="login">User's login.</param>
        /// <param name="password">User's password.</param>
        /// <returns>
        /// The task result contains <see cref="HttpResponseMessage"/>.
        /// </returns>
        Task<HttpResponseMessage> RegistrationAsync(string login, string password);

        /// <summary>
        /// Sends the request for logout from game;
        /// </summary>
        /// <returns>
        /// The task result contains <see cref="HttpResponseMessage"/>.
        /// </returns>
        Task<HttpResponseMessage> LogoutAsync();
    }
}
