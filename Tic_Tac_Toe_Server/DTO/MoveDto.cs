using System.Text.Json.Serialization;

namespace TicTacToe.Server.DTO
{
    public class MoveDto
    {
        [JsonPropertyName("indexOfCell")]
        public int IndexOfCell { get; set; }

        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonConstructor]
        public MoveDto(int indexOfCell, int number)
        {
            IndexOfCell = indexOfCell;
            Number = number;
        }
    }
}
