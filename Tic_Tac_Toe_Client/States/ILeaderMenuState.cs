using TicTacToe.Client.Enums;

namespace TicTacToe.Client.States
{
    public interface ILeaderMenuState : IState
    {
        /// <summary>
        /// Invoke <see cref="Services.IStatisticService.GetLeadersStatisticAsync(SortingType)"/>,
        /// processes response and show statistic.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>
        /// A task that represents the asynchronous operation. <see cref="Task"/>
        /// </returns>
        Task ShowLeadersStatistic(SortingType type);
    }
}
