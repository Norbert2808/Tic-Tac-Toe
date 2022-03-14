namespace TicTacToe.Server.Exceptions
{
    public class AccountException : Exception
    {
        public AccountException(string message)
            : base(message)
        {

        }

        public AccountException()
            : base()
        {

        }
    }
}
