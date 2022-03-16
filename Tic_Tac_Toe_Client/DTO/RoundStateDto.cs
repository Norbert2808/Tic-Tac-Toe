using System.Text.Json.Serialization;
using TicTacToe.Client.Models;

namespace TicTacToe.Client.DTO
{
    public sealed class RoundStateDto
    {
        [JsonPropertyName("board")]
        public List<Cell> Board { get; set; }

        [JsonPropertyName("isFirstPlayer")]
        public bool IsFirstPlayer { get; set; }

        [JsonPropertyName("isActiveFirstPlayer")]
        public bool IsActiveFirstPlayer { get; set; }

        [JsonPropertyName("isFinished")]
        public bool IsFinished { get; set; }

        [JsonConstructor]
        public RoundStateDto(List<Cell> board,
            bool isActiveFirst,
            bool isFirst,
            bool isFinished)
        {
            Board = board;
            IsActiveFirstPlayer = isActiveFirst;
            IsFirstPlayer = isFirst;
            IsFinished = isFinished;
        }
    }
}
