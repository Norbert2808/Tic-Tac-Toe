using System.Globalization;

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

        public static void ReadIntFromConsole(out int choose)
        {
            choose = Convert.ToInt32(Console.ReadLine(), CultureInfo.CurrentCulture);
        }

        public static void ReadStringFromConsole(out string str)
        {
            str = Console.ReadLine();
        }
    }
}
