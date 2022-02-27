using TicTacToe.Client.DTO;

namespace TicTacToe.Client.Models;

public class Board
{
    private int[] _boards = new int[9];


    public void Draw()
    {
        for (var i = 0; i <_boards.Length; i++)
        {
            if ( i % 3 == 0)
                Console.WriteLine();
            Console.Write(_boards[i] + " ");
        }
    }

    public void SetNumberByIndex(MoveDto move)
    {
        _boards[move.IndexOfCell] = move.Number;
    }
}
