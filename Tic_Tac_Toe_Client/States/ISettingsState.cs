

using TicTacToe.Client.Enums;

namespace TicTacToe.Client.States
{
    public interface ISettingsState : IState
    {
        void PopMenu(TimeOutType timeOutType);

        void GetValues(TimeOutType timeOutType, TimeType timeType);

        Task CloseMenuAsync();
    }
}
