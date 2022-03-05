namespace TicTacToe.Server.Exceptions;

class TimeOutException : Exception
{
    public TimeOutException(string message)
        : base(message)
    {
        
    }
}
