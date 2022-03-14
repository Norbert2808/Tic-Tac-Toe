namespace TicTacToe.Client.States
{
    internal interface IPrivateStatisticState : IState
    {
        Task GetPrivateStatistic();

        Task GetPrivateStatisticInTimeInterval();
    }
}
