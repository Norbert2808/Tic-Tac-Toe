namespace TicTacToe.Server.Exceptions
{
    public class GameException : Exception
    {
        public GameException(string message)
            : base(message)
        {

        }

        public GameException()
        {

        }
    }
}
