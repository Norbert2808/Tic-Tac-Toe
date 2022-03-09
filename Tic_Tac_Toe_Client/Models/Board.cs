using System.Globalization;
using TicTacToe.Client.DTO;

namespace TicTacToe.Client.Models
{
    public class Board
    {
        private List<Cell> _board = new(9);

        public void Draw()
        {
            Console.WriteLine(" —————————————————");

            DrawLineInBoard(_board[0].Value, ChooseColor(_board[0].IsFirstPlayer),
                _board[1].Value, ChooseColor(_board[1].IsFirstPlayer),
                _board[2].Value, ChooseColor(_board[2].IsFirstPlayer));
            Console.WriteLine("|_____|_____|_____|");

            DrawLineInBoard(_board[3].Value, ChooseColor(_board[3].IsFirstPlayer),
                _board[4].Value, ChooseColor(_board[4].IsFirstPlayer),
                _board[5].Value, ChooseColor(_board[5].IsFirstPlayer));
            Console.WriteLine("|_____|_____|_____|");

            DrawLineInBoard(_board[6].Value, ChooseColor(_board[6].IsFirstPlayer),
                _board[7].Value, ChooseColor(_board[7].IsFirstPlayer),
                _board[8].Value, ChooseColor(_board[8].IsFirstPlayer));

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
            _board[move.IndexOfCell] = new Cell(move.Number, isFirst);
        }

        public void SetBoard(List<Cell> board)
        {
            _board = board;
        }

    }
}
