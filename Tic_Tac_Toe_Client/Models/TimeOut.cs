
using System.Text.Json.Serialization;

namespace TicTacToe.Client.Models
{
    public class TimeOut
    {
        [JsonPropertyName("loginSettingsOwner")]
        public string LoginSettingsOwner { get; set; }

        [JsonPropertyName("roundTimeOut")]
        public TimeSpan RoundTimeOut { get; set; } = TimeSpan.FromSeconds(20);

        [JsonPropertyName("connectionTimeOut")]
        public TimeSpan ConnectionTimeOut { get; set; } = TimeSpan.FromMinutes(3);

        [JsonPropertyName("startGameTimeOut")]
        public TimeSpan StartGameTimeOut { get; set; } = TimeSpan.FromMinutes(1);

        [JsonPropertyName("roomActionTimeOut")]
        public TimeSpan RoomActionTimeOut { get; set; } = TimeSpan.FromMinutes(2);
    }
}
