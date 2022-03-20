using System.Text.Json.Serialization;
using TicTacToe.Server.Models;

namespace TicTacToe.Server.DTO
{
    public class RoundStateDto
    {
        [JsonPropertyName("board")]
        public List<Cell>? Board { get; set; }

        [JsonPropertyName("isFirstPlayer")]
        public bool IsFirstPlayer { get; set; }

        [JsonPropertyName("isActiveFirstPlayer")]
        public bool IsActiveFirstPlayer { get; set; }

        [JsonPropertyName("isFinished")]
        public bool IsFinished { get; set; }
    }
}
