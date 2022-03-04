using System.Text.Json.Serialization;

namespace TicTacToe.Server.Models
{
    public class Round
    {
        private readonly List<Tuple<int, bool?>> _board;

        [JsonPropertyName("FirstPlayerMove")]
        public List<Move> FirstPlayerMove { get; set; }

        [JsonPropertyName("SecondPlayerMove")]
        public List<Move> SecondPlayerMove { get; set; }

        public Move LasMove { get; private set; }

        public Round()
        {
            FirstPlayerMove = new List<Move>();
            SecondPlayerMove = new List<Move>();
            bool? player = null;
            _board = Enumerable.Repeat(new Tuple<int, bool?>(0, player), 9).ToList();
        }

        public bool DoMove(Move move, bool isFirst)
        {
            _board[move.IndexOfCell] = new Tuple<int, bool?>(move.Number, true);

            LasMove = move;

            if (isFirst)
            {
                FirstPlayerMove.Add(move);
            }
            else
            {
                SecondPlayerMove.Add(move);
            }

            return true;
        }

        public bool CheckEndOfGame(out bool? firstWin)
        {
            if (_board[0].Item2 is not null)
            {
                // first row
                if (_board[0].Item2 == _board[1].Item2 && _board[0].Item2 == _board[2].Item2)
                {
                    firstWin = _board[0].Item2;
                    return true;
                }
                // first colum
                if (_board[0].Item2 == _board[3].Item2 && _board[0].Item2 == _board[6].Item2)
                {
                    firstWin = _board[0].Item2;
                    return true;
                }
                // head diagonal
                if (_board[0].Item2 == _board[4].Item2 && _board[0].Item2 == _board[9].Item2)
                {
                    firstWin = _board[0].Item2;
                    return true;
                }
            }
            if (_board[4].Item2 is not null)
            {
                // second row
                if (_board[4].Item2 == _board[3].Item2 && _board[4].Item2 == _board[5].Item2)
                {
                    firstWin = _board[4].Item2;
                    return true;
                }
                // second colum
                if (_board[4].Item2 == _board[1].Item2 && _board[4].Item2 == _board[7].Item2)
                {
                    firstWin = _board[4].Item2;
                    return true;
                }
                // second diagonal
                if (_board[4].Item2 == _board[2].Item2 && _board[4].Item2 == _board[6].Item2)
                {
                    firstWin = _board[4].Item2;
                    return true;
                }
            }
            if (_board[8].Item2 is not null)
            {
                // third row
                if (_board[8].Item2 == _board[7].Item2 && _board[8].Item2 == _board[6].Item2)
                {
                    firstWin = _board[4].Item2;
                    return true;
                }
                // Third colum
                if (_board[8].Item2 == _board[5].Item2 && _board[8].Item2 == _board[2].Item2)
                {
                    firstWin = _board[4].Item2;
                    return true;
                }
            }

            firstWin = default;
            return false;
        }
    }
}
