using TicTacToe.Client.Enums;

namespace TicTacToe.Client.States
{
    public interface ILeaderMenuState : IState
    {
        Task ShowLeadersStatistic(SortingType type);
    }
}
