namespace Tic_Tac_Toe.Server.Tools
{
    public class UserBlocker : IBlocker
    {
        public int Counter { get; private set; }

        public bool IsBlocked()
        {
            return Counter == 3;
        }

        public void UnBlock()
        {
            Counter = 0;
        }

        public void WrongTryEntry()
        {
            Counter++;
        }
    }
}
