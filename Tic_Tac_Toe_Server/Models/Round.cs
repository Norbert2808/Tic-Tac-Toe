using System.Text.Json.Serialization;
using TicTacToe.Server.DTO;

namespace TicTacToe.Server.Models
{
    public class Round
    {
        [JsonIgnore]
        public List<Cell> Board { get; private set; }

        [JsonPropertyName("firstPlayerMove")]
        public List<MoveDto> FirstPlayerMove { get; set; }

        [JsonPropertyName("secondPlayerMove")]
        public List<MoveDto> SecondPlayerMove { get; set; }

        [JsonIgnore]
        public HashSet<int> AvailableValueFirstPlayerNumbers { get; private set; }

        [JsonIgnore]
        public HashSet<int> AvailableValueSecondPlayerNumbers { get; private set; }

        [JsonIgnore]
        public MoveDto? LastMove { get; set; }

        [JsonIgnore]
        private readonly object _locker = new();

        [JsonIgnore]
        public bool IsActiveFirstPlayer { get; set; }

        [JsonIgnore]
        public bool IsFinished { get; set; }

        public Round()
        {
            FirstPlayerMove = new List<MoveDto>();
            SecondPlayerMove = new List<MoveDto>();
            bool? player = null;
            Board = Enumerable.Repeat(new Cell(0, player), 9).ToList();
            AvailableValueFirstPlayerNumbers = Enumerable.Range(1, 9).ToHashSet();
            AvailableValueSecondPlayerNumbers = Enumerable.Range(1, 9).ToHashSet();
            IsFinished = false;
        }

        [JsonConstructor]
        public Round(List<MoveDto> firstPlayerMove,
            List<MoveDto> secondPlayerMove)
        {
            FirstPlayerMove = firstPlayerMove;
            SecondPlayerMove = secondPlayerMove;
            bool? player = null;
            Board = Enumerable.Repeat(new Cell(0, player), 9).ToList();
            AvailableValueFirstPlayerNumbers = Enumerable.Range(1, 9).ToHashSet();
            AvailableValueSecondPlayerNumbers = Enumerable.Range(1, 9).ToHashSet();
        }

        public bool DoMove(MoveDto move, bool isFirst)
        {
            lock (_locker)
            {
                MovingValidation(move, isFirst);

                Board[move.IndexOfCell] = new Cell(move.Number, isFirst);

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
                if (Board[0].IsFirstPlayer is not null)
                {
                    // first row
                    if (Board[0].IsFirstPlayer == Board[1].IsFirstPlayer && Board[0].IsFirstPlayer == Board[2].IsFirstPlayer)
                    {
                        return true;
                    }

                    // first colum
                    if (Board[0].IsFirstPlayer == Board[3].IsFirstPlayer && Board[0].IsFirstPlayer == Board[6].IsFirstPlayer)
                    {
                        return true;
                    }

                    // head diagonal
                    if (Board[0].IsFirstPlayer == Board[4].IsFirstPlayer && Board[0].IsFirstPlayer == Board[8].IsFirstPlayer)
                    {
                        return true;
                    }
                }

                if (Board[4].IsFirstPlayer is not null)
                {
                    // second row
                    if (Board[4].IsFirstPlayer == Board[3].IsFirstPlayer && Board[4].IsFirstPlayer == Board[5].IsFirstPlayer)
                    {
                        return true;
                    }

                    // second colum
                    if (Board[4].IsFirstPlayer == Board[1].IsFirstPlayer && Board[4].IsFirstPlayer == Board[7].IsFirstPlayer)
                    {
                        return true;
                    }

                    // second diagonal
                    if (Board[4].IsFirstPlayer == Board[2].IsFirstPlayer && Board[4].IsFirstPlayer == Board[6].IsFirstPlayer)
                    {
                        return true;
                    }
                }

                if (Board[8].IsFirstPlayer is not null)
                {
                    // third row
                    if (Board[8].IsFirstPlayer == Board[7].IsFirstPlayer && Board[8].IsFirstPlayer == Board[6].IsFirstPlayer)
                    {
                        return true;
                    }

                    // third column
                    if (Board[8].IsFirstPlayer == Board[5].IsFirstPlayer && Board[8].IsFirstPlayer == Board[2].IsFirstPlayer)
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
            if (Board[move.IndexOfCell].Value >= move.Number)
                throw new ArgumentException("This cell contains a greater or equal number");
            if (Board[move.IndexOfCell].IsFirstPlayer == isFirst)
                throw new ArgumentException("You can't change your cell");

            if (isFirst)
            {
                if (!AvailableValueFirstPlayerNumbers.Contains(move.Number))
                {
                    var unusedNumbers = string.Join(" ", AvailableValueFirstPlayerNumbers.Select(x => x.ToString()));
                    throw new ArgumentException($"You have already used the number: {move.Number}" +
                        $"\nYou have numbers: {unusedNumbers}");
                }
                _ = AvailableValueFirstPlayerNumbers.Remove(move.Number);
            }
            else
            {
                if (!AvailableValueSecondPlayerNumbers.Contains(move.Number))
                {
                    var unusedNumbers = string.Join(" ", AvailableValueSecondPlayerNumbers.Select(x => x.ToString()));
                    throw new ArgumentException($"You have already used the number: {move.Number}" +
                        $"\nYou have numbers: {unusedNumbers}");
                }
                _ = AvailableValueSecondPlayerNumbers.Remove(move.Number);
            }
        }

        public MoveDto GetValidMoveFromBot()
        {
            var random = new Random();

            var validIndexList = Board
                    .Where((cell, index) => cell.Value == 0 || cell.IsFirstPlayer == true)
                    .Select((cell, index) => index)
                    .ToList();

            while (true)
            {
                var randomIndex = validIndexList.ElementAt(random.Next(validIndexList.Count));

                if (Board[randomIndex].IsFirstPlayer == true)
                {
                    var opponentValue = Board[randomIndex].Value;
                    var greaterValues = AvailableValueSecondPlayerNumbers
                        .Where(value => value > opponentValue)
                        .ToList();
                    if (greaterValues.Count != 0)
                    {
                        var randomValue = greaterValues
                            .ElementAt(random.Next(greaterValues.Count));
                        return new MoveDto(randomIndex, randomValue);
                    }
                }
                else
                {
                    var randomValue = AvailableValueSecondPlayerNumbers
                        .ElementAt(random.Next(AvailableValueSecondPlayerNumbers.Count));
                    return new MoveDto(randomIndex, randomValue);
                }
            }
        }
    }
}
