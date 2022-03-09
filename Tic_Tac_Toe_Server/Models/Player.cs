using System.Text.Json.Serialization;

namespace TicTacToe.Server.Models
{
    public sealed class Player
    {
        [JsonPropertyName("login")]
        public string Login { get; set; }

        [JsonPropertyName("wins")]
        public int Wins { get; set; }

        public Player(string login, int wins)
        {
            Login = login;
            Wins = wins;
        }
    }
}
