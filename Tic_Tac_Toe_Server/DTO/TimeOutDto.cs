using System.Text.Json.Serialization;

namespace TicTacToe.Server.DTO
{
    public class TimeOutDto
    {
        [JsonPropertyName("loginSettingsOwner")]
        public string LoginSettingsOwner { get; set; }

        [JsonPropertyName("roundTimeOut")]
        public TimeSpan RoundTimeOut { get; set; }

        [JsonPropertyName("connectionTimeOut")]
        public TimeSpan ConnectionTimeOut { get; set; }

        [JsonPropertyName("startGameTimeOut")]
        public TimeSpan StartGameTimeOut { get; set; }

        [JsonPropertyName("roomActionTimeOut")]
        public TimeSpan RoomActionTimeOut { get; set; }
    }
}
