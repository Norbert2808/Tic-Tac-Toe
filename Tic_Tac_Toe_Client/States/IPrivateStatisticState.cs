namespace TicTacToe.Client.States
{
    internal interface IPrivateStatisticState : IState
    {
        /// <summary>
        /// Sends the request to get private statistic.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. <see cref="Task"/>
        /// </returns>
        Task GetPrivateStatisticAsync();

        /// <summary>
        /// Sends the request to get private statistic in time interval.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. <see cref="Task"/>
        /// </returns>
        Task GetPrivateStatisticInTimeIntervalAsync();
    }
}
