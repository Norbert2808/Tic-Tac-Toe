namespace TicTacToe.Server.CustomLogger
{
    public class LoggerProvider : ILoggerProvider
    {
        private FileLogger? _fileLogger;

        private readonly string _path;

        private bool _disposed;

        public LoggerProvider(string path)
        {
            _path = path;
        }

        public ILogger CreateLogger(string categoryName)
        {
            _fileLogger = new FileLogger(_path);
            return _fileLogger;
        }

#pragma warning disable CA1816
        public void Dispose()
#pragma warning restore CA1816
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                _fileLogger = null!;
            }

            _disposed = true;
        }

    }
}
