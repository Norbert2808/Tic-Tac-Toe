using System.Text.Json.Serialization;

namespace TicTacToe.Server.Models
{
    public class PrivateStatisticDto
    {
        [JsonPropertyName("winnings")]
        public int Winnings { get; set; }

        [JsonPropertyName("losses")]
        public int Losses { get; set; }

        [JsonPropertyName("totalNumberOfRooms")]
        public int TotalNumberOfRooms { get; set; }

        [JsonPropertyName("totalNumberOfMoves")]
        public int TotalNumberOfMoves { get; set; }

        [JsonPropertyName("mostUsedNumbers")]
        public List<int> MostUsedNumbers { get; set; }

        [JsonPropertyName("mostNumbersCount")]
        public int MostNumbersCount { get; set; }

        [JsonPropertyName("mostUsedPosition")]
        public List<int> MostUsedPosition { get; set; }

        [JsonPropertyName("mostPositionCount")]
        public int MostPositionCount { get; set; }

        [JsonPropertyName("allTimeInGame")]
        public TimeSpan AllTimeInGame { get; set; }

        public PrivateStatisticDto(int winnings,
            int losses,
            int totalNumberOfGames,
            int totalNumberOfMoves,
            List<int> mostUsedNumbers,
            int mostNumbersCount,
            List<int> mostUsedPosition,
            int mostPositionCount,
            TimeSpan allTimeInGame)
        {
            Winnings = winnings;
            Losses = losses;
            TotalNumberOfRooms = totalNumberOfGames;
            TotalNumberOfMoves = totalNumberOfMoves;
            MostUsedNumbers = mostUsedNumbers;
            MostNumbersCount = mostNumbersCount;
            MostUsedPosition = mostUsedPosition;
            MostPositionCount = mostPositionCount;
            AllTimeInGame = allTimeInGame;
        }
    }
}
