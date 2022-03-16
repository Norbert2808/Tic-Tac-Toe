

using TicTacToe.Client.Enums;

namespace TicTacToe.Client.States
{
    public interface ISettingsState : IState
    {
        void PopMenu(TimeOutType timeOutType);

        void GetValues(TimeOutType timeOutType, TimeType timeType);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. <see cref="Task"/>
        /// </returns>
        Task CloseMenuAsync();
    }
}
