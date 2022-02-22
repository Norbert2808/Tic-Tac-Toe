namespace Tic_Tac_Toe.Server.Tools
{
    public interface IBlocker
    {
        bool IsBlocked();

        void UnBlock();

        void WrongTryEntry();
    }
}
