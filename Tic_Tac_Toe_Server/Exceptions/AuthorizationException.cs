namespace TicTacToe.Server.Exceptions
{
    public class AuthorizationException : Exception
    {
        public AuthorizationException(string message)
            : base(message)
        {

        }

        public AuthorizationException()
            : base()
        {

        }
    }
}
