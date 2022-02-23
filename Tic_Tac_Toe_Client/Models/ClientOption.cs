using System.Text.Json.Serialization;

namespace TicTacToe.Client.Models
{
    internal class ClientOption
    {
        [JsonPropertyName("BaseAddress")]
        public string UriAddress { get; set; }
    }
}
