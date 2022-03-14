using System.Globalization;
using TicTacToe.Client.Tools;

namespace TicTacToe.Client.Models
{
    public sealed class Board
    {
        private List<Cell> _board = new(9);

        /// <summary>
        /// Draw playing field
        /// </summary>
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

        /// <summary>
        /// Choose color for player.
        /// If this is first player -- green color.
        /// If this is second player -- red color.
        /// </summary>
        /// <param name="isFirst">Variable that shows which player by number</param>
        /// <returns><see cref="ConsoleColor"/></returns>
        private ConsoleColor ChooseColor(bool? isFirst)
        {
            return isFirst is null
                ? ConsoleColor.White
                : (bool)isFirst
                ? ConsoleColor.Green
                : ConsoleColor.Red;
        }

        /// <summary>
        /// Draw lines for playing field.
        /// </summary>
        /// <param name="firstNumber">First number in the line</param>
        /// <param name="firstNumColor">Color first number in the line</param>
        /// <param name="secondNumber">Second number in the line</param>
        /// <param name="secondNumColor">Color second number in the line</param>
        /// <param name="thirdNumber">Third number in the line</param>
        /// <param name="thirdNumColor">Color third number in the line</param>
        private void DrawLineInBoard(int firstNumber, ConsoleColor firstNumColor,
            int secondNumber, ConsoleColor secondNumColor,
            int thirdNumber, ConsoleColor thirdNumColor)
        {
            Console.WriteLine("|     |     |     |");
            Console.Write("|  ");
            ConsoleHelper.WriteInConsole(firstNumber.ToString(CultureInfo.InvariantCulture), firstNumColor);
            Console.Write("  |  ");
            ConsoleHelper.WriteInConsole(secondNumber.ToString(CultureInfo.InvariantCulture), secondNumColor);
            Console.Write("  |  ");
            ConsoleHelper.WriteInConsole(thirdNumber.ToString(CultureInfo.InvariantCulture), thirdNumColor);
            Console.WriteLine("  |");
        }

        /// <summary>
        /// Set a new board instance 
        /// </summary>
        /// <param name="board">New board</param>
        public void SetBoard(List<Cell> board)
        {
            _board = board;
        }

    }
}
