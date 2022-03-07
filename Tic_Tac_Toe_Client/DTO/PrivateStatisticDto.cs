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

        [JsonPropertyName("mostUsedPosition")]
        public List<int> MostUsedPosition { get; set; }

        public PrivateStatisticDto(int winnings, int losses,
            List<int> mostNumbers, List<int> mostPosition)
        {
            Winnings = winnings;
            Losses = losses;
            MostUsedNumbers = mostNumbers;
            MostUsedPosition = mostPosition;
        }
    }
}
