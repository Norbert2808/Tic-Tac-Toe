namespace Tic_Tac_Toe.Server.CustomLogger
{
    public static class LoggerExtensions
    {
        public static ILoggingBuilder AddFile(this ILoggingBuilder factory, string path)
        {
            _ = factory ?? throw new ArgumentNullException(nameof(factory));

            return factory.AddProvider(new LoggerProvider(path));
        }

    }
}
