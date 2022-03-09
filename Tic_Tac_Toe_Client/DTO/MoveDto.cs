using System.Text.Json.Serialization;

namespace TicTacToe.Client.DTO
{
    public sealed class MoveDto
    {
        [JsonPropertyName("indexOfCell")]
        public int IndexOfCell { get; set; }

        [JsonPropertyName("number")]
        public int Number { get; set; }

        public MoveDto(int indexOfCell, int number)
        {
            IndexOfCell = indexOfCell;
            Number = number;
        }
    }
}
