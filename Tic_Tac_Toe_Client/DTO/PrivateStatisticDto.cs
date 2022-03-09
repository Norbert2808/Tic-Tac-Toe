using System.Text.Json.Serialization;

namespace TicTacToe.Client.DTO
{
    internal class PrivateStatisticDto
    {
        [JsonPropertyName("winnings")]
        public int Winnings { get; set; }

        [JsonPropertyName("losses")]
        public int Losses { get; set; }

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

        [JsonConstructor]
        public PrivateStatisticDto(int winnings,
            int losses,
            List<int> mostUsedNumbers,
            int mostNumbersCount,
            List<int> mostUsedPosition,
            int mostPositionCount,
            TimeSpan allTimeInGame)
        {
            Winnings = winnings;
            Losses = losses;
            MostUsedNumbers = mostUsedNumbers;
            MostNumbersCount = mostNumbersCount;
            MostUsedPosition = mostUsedPosition;
            MostPositionCount = mostPositionCount;
            AllTimeInGame = allTimeInGame;
        }
    }
}
