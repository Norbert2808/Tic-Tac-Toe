namespace Tic_Tac_Toe.Client
{
    public static class ConsoleHelper
    {
        public static void WriteInConsole(string[] message,
            ConsoleColor color)
        {
            Console.Clear();
            var startColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            foreach (var item in message)
                Console.WriteLine(item);
            Console.WriteLine("Please press to continue");

            Console.ForegroundColor = startColor;

            _ = Console.ReadKey();
        }
    }
}
