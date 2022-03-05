namespace TicTacToe.Server.Exceptions;

class AuthorizationException : Exception
{
    public AuthorizationException(string message)
        : base(message)
    {
        
    }
}
