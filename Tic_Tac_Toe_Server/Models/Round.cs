using System.Text.Json.Serialization;

namespace TicTacToe.Server.Models
{
    public class Round
    {
        [JsonIgnore]
        private readonly List<Tuple<int, bool?>> _board;

        [JsonPropertyName("firstPlayerMove")]
        public List<MoveDto> FirstPlayerMove { get; set; }

        [JsonPropertyName("secondPlayerMove")]
        public List<MoveDto> SecondPlayerMove { get; set; }

        [JsonPropertyName("firstWin")]
        public bool FirstWin { get; set; }

        [JsonIgnore]
        private HashSet<int> FirstPlayerNumbers { get; set; }

        [JsonIgnore]
        private HashSet<int> SecondPlayerNumbers { get; set; }

        [JsonIgnore]
        public MoveDto? LastMove { get; set; }

        [JsonIgnore]
        private readonly object _locker = new();

        [JsonIgnore]
        public bool IsStarted { get; set; }

        public Round()
        {
            FirstPlayerMove = new List<MoveDto>();
            SecondPlayerMove = new List<MoveDto>();
            bool? player = null;
            _board = Enumerable.Repeat(new Tuple<int, bool?>(0, player), 9).ToList();
            FirstPlayerNumbers = Enumerable.Range(1, 9).ToHashSet();
            SecondPlayerNumbers = Enumerable.Range(1, 9).ToHashSet();
            IsStarted = true;
        }

        [JsonConstructor]
        public Round(List<MoveDto> firstPlayerMove,
            List<MoveDto> secondPlayerMove,
            bool firstWin)
        {
            FirstPlayerMove = firstPlayerMove;
            SecondPlayerMove = secondPlayerMove;
            FirstWin = firstWin;
            bool? player = null;
            _board = Enumerable.Repeat(new Tuple<int, bool?>(0, player), 9).ToList();
            FirstPlayerNumbers = Enumerable.Range(1, 9).ToHashSet();
            SecondPlayerNumbers = Enumerable.Range(1, 9).ToHashSet();
        }

        public bool DoMove(MoveDto move, bool isFirst)
        {
            lock (_locker)
            {
                MovingValidation(move, isFirst);

                _board[move.IndexOfCell] = new Tuple<int, bool?>(move.Number, isFirst);

                LastMove = move;

                if (isFirst)
                {
                    FirstPlayerMove.Add(move);
                }
                else
                {
                    SecondPlayerMove.Add(move);
                }
            }

            return true;
        }

        public bool CheckOpponentsMove(bool isFirst)
        {
            return isFirst
                ? FirstPlayerMove.Count == SecondPlayerMove.Count
                : FirstPlayerMove.Count - 1 == SecondPlayerMove.Count;
        }

        public bool CheckEndOfGame()
        {
            lock (_locker)
            {
                if (_board[0].Item2 is not null)
                {
                    // first row
                    if (_board[0].Item2 == _board[1].Item2 && _board[0].Item2 == _board[2].Item2)
                    {
                        return true;
                    }

                    // first colum
                    if (_board[0].Item2 == _board[3].Item2 && _board[0].Item2 == _board[6].Item2)
                    {
                        return true;
                    }

                    // head diagonal
                    if (_board[0].Item2 == _board[4].Item2 && _board[0].Item2 == _board[8].Item2)
                    {
                        return true;
                    }
                }

                if (_board[4].Item2 is not null)
                {
                    // second row
                    if (_board[4].Item2 == _board[3].Item2 && _board[4].Item2 == _board[5].Item2)
                    {
                        return true;
                    }

                    // second colum
                    if (_board[4].Item2 == _board[1].Item2 && _board[4].Item2 == _board[7].Item2)
                    {
                        return true;
                    }

                    // second diagonal
                    if (_board[4].Item2 == _board[2].Item2 && _board[4].Item2 == _board[6].Item2)
                    {
                        return true;
                    }
                }

                if (_board[8].Item2 is not null)
                {
                    // third row
                    if (_board[8].Item2 == _board[7].Item2 && _board[8].Item2 == _board[6].Item2)
                    {
                        return true;
                    }

                    // third column
                    if (_board[8].Item2 == _board[5].Item2 && _board[8].Item2 == _board[2].Item2)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private void MovingValidation(MoveDto move, bool isFirst)
        {
            if (move.IndexOfCell is < 0 or > 8)
                throw new ArgumentException("Cell number must be in range [1;9]");
            if (_board[move.IndexOfCell].Item1 >= move.Number)
                throw new ArgumentException("This cell contains a greater or equal number");
            if (_board[move.IndexOfCell].Item2 == isFirst)
                throw new ArgumentException("You can't change your cell");

            if (isFirst)
            {
                if (!FirstPlayerNumbers.Contains(move.Number))
                {
                    var unusedNumbers = string.Join(" ", FirstPlayerNumbers.Select(x => x.ToString()));
                    throw new ArgumentException($"You have already used the number: {move.Number}" +
                        $"\nYou have numbers: {unusedNumbers}");
                }
                _ = FirstPlayerNumbers.Remove(move.Number);
            }
            else
            {
                if (!SecondPlayerNumbers.Contains(move.Number))
                {
                    var unusedNumbers = string.Join(" ", SecondPlayerNumbers.Select(x => x.ToString()));
                    throw new ArgumentException($"You have already used the number: {move.Number}" +
                        $"\nYou have numbers: {unusedNumbers}");
                }
                _ = SecondPlayerNumbers.Remove(move.Number);
            }
        }
    }
}
