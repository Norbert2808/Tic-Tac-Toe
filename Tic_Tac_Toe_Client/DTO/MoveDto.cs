namespace TicTacToe.Client.DTO;

public class MoveDto
{
    public int IndexOfCell { get; set; }

    public int Number { get; set; }

    public MoveDto(int index, int number)
    {
        IndexOfCell = index;
        Number = number;
    }
}
