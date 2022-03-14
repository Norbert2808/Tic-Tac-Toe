
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
    }
}
