using System.Text.Json.Serialization;

namespace TicTacToe.Server.Models;

public class Round
{
    private readonly List<Tuple<int, bool>> _board;
    
    [JsonPropertyName("FirstPlayerMove")]
    public List<Move> FirstPlayerMove { get; set; }

    [JsonPropertyName("SecondPlayerMove")]
    public List<Move> SecondPlayerMove { get; set; }

    public Move LasMove { get; private set; }

    public Round()
    {
        FirstPlayerMove = new List<Move>();
        SecondPlayerMove = new List<Move>();
        _board = Enumerable.Repeat(new Tuple<int, bool>(0, false), 9).ToList();
    }

    public bool DoMove(Move move, bool isFirst)
    {
        _board[move.IndexOfCell] = new Tuple<int, bool>(move.Number, true);

        LasMove = move;
        
        if (isFirst)
            FirstPlayerMove.Add(move);
        else
        {
            SecondPlayerMove.Add(move);
        }
        
        return true;
    }
}
