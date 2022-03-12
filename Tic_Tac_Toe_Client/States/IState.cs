
namespace TicTacToe.Client.States
{
    public interface IState
    {
        /// <summary>
        /// Invoke main menu.
        /// </summary>
        /// <returns>
        /// Return <see cref="Task"/>
        /// </returns>
        Task InvokeMenuAsync();
    }
}
