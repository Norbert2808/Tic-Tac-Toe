using System.Text.Json.Serialization;


namespace Tic_Tac_Toe.Client.Models
{
    internal class ClientOption
    {
        [JsonPropertyName("BaseAddress")]
        public string UriAddress { get; set; }
    }
}
