namespace TicTacToe.Client.Services
{
    public interface IStatisticService
    {
        Task<HttpResponseMessage> GetPrivateStatisticDto();
    }
}
