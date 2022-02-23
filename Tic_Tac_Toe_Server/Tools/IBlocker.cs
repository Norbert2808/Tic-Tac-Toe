namespace TicTacToe.Server.Tools
{
    public interface IBlocker
    {
        void UnBlock(string login);

        bool IsBlocked(string login);

        void ErrorTryLogin(string login);
    }
}
