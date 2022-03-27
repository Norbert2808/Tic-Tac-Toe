
using System.Text.Json.Serialization;

namespace TicTacToe.Client.DTO
{
    public sealed class UserDto
    {
        [JsonPropertyName("login")]
        public string? Login { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }
}
