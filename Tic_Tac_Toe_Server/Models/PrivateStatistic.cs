using System.Text.Json.Serialization;

namespace TicTacToe.Server.Models
{
    public class PrivateStatistic
    {
        [JsonPropertyName("winnings")]
        public int Winnings { get; set; }

        [JsonPropertyName("losses")]
        public int Losses { get; set; }

        [JsonPropertyName("mostUsedNumbers")]
        public List<int> MostUsedNumbers { get; set; }

        [JsonPropertyName("mostUsedPosition")]
        public List<int> MostUsedPosition { get; set; }

        public PrivateStatistic(int winnings, int losses,
            List<int> mostUsedNumbers, List<int> mostUsedPosition)
        {
            Winnings = winnings;
            Losses = losses;
            MostUsedNumbers = mostUsedNumbers;
            MostUsedPosition = mostUsedPosition;
        }
    }
}
