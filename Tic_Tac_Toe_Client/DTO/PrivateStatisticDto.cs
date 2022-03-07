using System.Text.Json.Serialization;

namespace TicTacToe.Client.DTO
{
    internal class PrivateStatisticDto
    {
        [JsonPropertyName("winnings")]
        public int Winnings { get; set; }

        [JsonPropertyName("losses")]
        public int Losses { get; set; }

        [JsonPropertyName("moves")]
        public List<MoveDto>? Moves { get; set; }

    }
}
