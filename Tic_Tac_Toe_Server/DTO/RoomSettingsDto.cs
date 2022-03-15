using System.Text.Json.Serialization;
using TicTacToe.Server.DTO;
using TicTacToe.Server.Enums;

namespace TicTacToe.Server.Models
{
    public class RoomSettingsDto
    { 
        [JsonPropertyName("type")]
        public RoomType Type { get; set; }

        [JsonPropertyName("roomId")]
        public string RoomId { get; set; }

        [JsonPropertyName("isConnection")]
        public bool IsConnection { get; set; }

        [JsonPropertyName("times")]
        public TimeOutDto TimeOut { get; set; }
    }
}
