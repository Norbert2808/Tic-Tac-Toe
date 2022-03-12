namespace TicTacToe.Client.States
{
    public interface IAuthorizationMenuState : IState
    {
        /// <summary>
        /// Execute login menu and invoke
        /// <see cref="Services.IUserService.LoginAsync(string, string)"/>,
        /// also processes the response.
        /// </summary>
        /// <returns>
        /// Returns <see cref="Task"/>
        /// </returns>
        Task ExecuteLoginAsync();

        /// <summary>
        /// Execute registration menu and invoke
        /// <see cref="Services.IUserService.RegistrationAsync(string, string)"/>
        /// also processes the response.
        /// </summary>
        /// <returns>
        /// Returns <see cref="Task"/>
        /// </returns>
        Task ExecuteRegistrationAsync();
    }
}
