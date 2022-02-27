namespace TicTacToe.Server.Models;

public class Move
{
    public int IndexOfCell { get; set; }

    public int Number { get; set; }

    public Move(int index, int number)
    {
        IndexOfCell = index;
        Number = number;
    }
}
