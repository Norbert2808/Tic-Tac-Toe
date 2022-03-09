using System.Text.Json.Serialization;

namespace TicTacToe.Server.Models
{
    public sealed class Cell
    {
        [JsonPropertyName("value")]
        public int Value { get; set; }

        [JsonPropertyName("isFirstPlayer")]
        public bool? IsFirstPlayer { get; set; }

        [JsonConstructor]
        public Cell(int value, bool? isFirst)
        {
            Value = value;
            IsFirstPlayer = isFirst;
        }
    }
}
