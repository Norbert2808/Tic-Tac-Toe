namespace TicTacToe.Server.CustomLogger
{
    public class FileLogger : ILogger
    {
        private readonly string _filePath;

        private static readonly object _lock = new();

        public FileLogger(string path)
        {
            _filePath = path;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string>? formatter)
        {
            if (formatter is null)
                return;
            lock (_lock)
            {
                File.AppendAllText(_filePath, formatter(state, exception!) + Environment.NewLine);
            }

        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return default;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return default!;
        }
    }
}
