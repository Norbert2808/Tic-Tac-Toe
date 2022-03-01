using System.Globalization;
using TicTacToe.Client.DTO;

namespace TicTacToe.Client.Models;

public class Board
{
    private int[] _boards = new int[9];


    public void Draw()
    {
        Console.WriteLine(" —————————————————");

        DrawLineInBoard(_boards[0], ConsoleColor.Red,
            _boards[1], ConsoleColor.Red,
            _boards[2], ConsoleColor.Green);
        Console.WriteLine("|_____|_____|_____|");

        DrawLineInBoard(_boards[3], ConsoleColor.Green,
            _boards[4], ConsoleColor.Red,
            _boards[5], ConsoleColor.Green);
        Console.WriteLine("|_____|_____|_____|");

        DrawLineInBoard(_boards[6], ConsoleColor.Red,
            _boards[7], ConsoleColor.Green,
            _boards[8], ConsoleColor.Red);

        Console.WriteLine("|     |     |     |");
        Console.WriteLine(" —————————————————");
    }

    private void DrawLineInBoard(int x, ConsoleColor xColor,
        int y, ConsoleColor yColor,
        int z, ConsoleColor zColor)
    {
        Console.WriteLine("|     |     |     |");
        Console.Write("|  ");
        ConsoleHelper.WriteInConsole(x.ToString(CultureInfo.InvariantCulture), xColor);
        Console.Write("  |  ");
        ConsoleHelper.WriteInConsole(y.ToString(CultureInfo.InvariantCulture), yColor);
        Console.Write("  |  ");
        ConsoleHelper.WriteInConsole(z.ToString(CultureInfo.InvariantCulture), zColor);
        Console.WriteLine("  |");
    }

    public void SetNumberByIndex(MoveDto move)
    {
        _boards[move.IndexOfCell] = move.Number;
    }
}
