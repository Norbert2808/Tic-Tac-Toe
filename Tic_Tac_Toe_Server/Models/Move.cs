using System.Text.Json.Serialization;

namespace TicTacToe.Server.Models
{
    public class Move
    {
        [JsonPropertyName("indexOfCell")]
        public int IndexOfCell { get; set; }

        [JsonPropertyName("number")]
        public int Number { get; set; }

        public Move(int indexOfCell, int number)
        {
            IndexOfCell = indexOfCell;
            Number = number;
        }
    }
}
