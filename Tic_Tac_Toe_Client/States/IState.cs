
namespace TicTacToe.Client.States
{
    public interface IState
    {
        /// <summary>
        /// Invoke main menu.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. <see cref="Task"/>
        /// </returns>
        Task InvokeMenuAsync();

        /// <summary>
        /// Logging information by Serilog.
        /// </summary>
        /// <param name="methodName">Method name.</param>
        /// <param name="message">Some message.</param>
        void LogInformationAboutClass(string methodName, string message);
    }
}
