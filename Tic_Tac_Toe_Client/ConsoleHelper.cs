using System.Globalization;

namespace TicTacToe.Client
{
    public static class ConsoleHelper
    {
        public static void WriteInConsole(string[] message,
            ConsoleColor color,
            string lastMessage = "Please press to continue")
        {
            Console.Clear();
            var startColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            foreach (var item in message)
                Console.WriteLine(item);
            Console.WriteLine(lastMessage);

            Console.ForegroundColor = startColor;
            _ = Console.ReadKey();
        }
        
        public static void WriteInConsoleWithOutReadLine(string[] message,
            ConsoleColor color)
        {
            Console.Clear();
            var startColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            foreach (var item in message)
                Console.WriteLine(item);

            Console.ForegroundColor = startColor;
        }

        public static void ReadIntFromConsole(out int choose)
        {
            choose = Convert.ToInt32(Console.ReadLine(), CultureInfo.CurrentCulture);
        }
    }
}
