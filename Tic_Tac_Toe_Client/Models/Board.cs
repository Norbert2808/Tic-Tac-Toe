using System.Globalization;
using TicTacToe.Client.DTO;

namespace TicTacToe.Client.Models;

public class Board
{
    private List<Tuple<int, bool?>> _board = new(9);
    
    public void Draw()
    {
        Console.WriteLine(" —————————————————");

        DrawLineInBoard(_board[0].Item1, ChooseColor(_board[0].Item2),
            _board[1].Item1, ChooseColor(_board[1].Item2),
            _board[2].Item1, ChooseColor(_board[2].Item2));
        Console.WriteLine("|_____|_____|_____|");

        DrawLineInBoard(_board[3].Item1, ChooseColor(_board[3].Item2),
            _board[4].Item1, ChooseColor(_board[4].Item2),
            _board[5].Item1, ChooseColor(_board[5].Item2));
        Console.WriteLine("|_____|_____|_____|");

        DrawLineInBoard(_board[6].Item1, ChooseColor(_board[6].Item2),
            _board[7].Item1, ChooseColor(_board[7].Item2),
            _board[8].Item1, ChooseColor(_board[8].Item2));

        Console.WriteLine("|     |     |     |");
        Console.WriteLine(" —————————————————");
    }

    private ConsoleColor ChooseColor(bool? isFirst)
    {
        return isFirst is null
            ? ConsoleColor.White
            : (bool)isFirst
            ? ConsoleColor.Green
            : ConsoleColor.Red;
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

    public void SetNumberByIndex(MoveDto move, bool isFirst)
    {
        _board[move.IndexOfCell] = new Tuple<int, bool?>(move.Number, isFirst);
    }

    public void SetDefaultValuesInBoard()
    {
        bool? player = null;
        _board = Enumerable.Repeat(new Tuple<int, bool?>(0, player), 9).ToList();
    }
}
