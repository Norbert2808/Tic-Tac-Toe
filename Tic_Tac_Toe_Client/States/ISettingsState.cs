

using TicTacToe.Client.Enums;

namespace TicTacToe.Client.States
{
    public interface ISettingsState : IState
    {
        /// <summary>
        ///  Invoke second menu for choice type of time.
        /// </summary>
        /// <param name="timeOutType">Type of time out <see cref="Enums.TimeOutType"/>.</param>
        void PopTimeMenu(TimeOutType timeOutType);

        /// <summary>
        /// Save correct input data in <see cref="Models.TimeOut"/>
        /// </summary>
        /// <param name="timeOutType">Type of time out <see cref="Enums.TimeOutType"/>.</param>
        /// <param name="timeType">Type of time <see cref="Enums.TimeType"/>.</param>
        void SetValueToTimeOut(TimeOutType timeOutType, TimeType timeType);

        /// <summary>
        ///  Close settings state and save options in json file.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. <see cref="Task"/>
        /// </returns>
        Task CloseMenuAsync();
    }
}
