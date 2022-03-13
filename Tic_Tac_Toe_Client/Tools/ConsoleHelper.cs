using System.Globalization;

namespace TicTacToe.Client.Tools
{
    public static class ConsoleHelper
    {
        public static void WriteInConsole(string[] message,
            ConsoleColor color,
            string lastMessage = "Please press to continue")
        {
            var startColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            foreach (var item in message)
                Console.WriteLine(item);
            Console.WriteLine(lastMessage);

            Console.ForegroundColor = startColor;
        }

        public static void WriteInConsole(string message,
            ConsoleColor color)
        {
            var startColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            Console.Write(message);

            Console.ForegroundColor = startColor;
        }

        public static void ReadIntFromConsole(out int choose)
        {
            choose = Convert.ToInt32(Console.ReadLine(), CultureInfo.CurrentCulture);
        }
    }
}
