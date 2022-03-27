using System.Text.Json.Serialization;
using TicTacToe.Client.Enums;
using TicTacToe.Client.Models;

namespace TicTacToe.Client.DTO
{
    public sealed class RoomSettingsDto
    {
        [JsonPropertyName("type")]
        public RoomType Type { get; set; }

        [JsonPropertyName("roomId")]
        public string RoomId { get; set; }

        [JsonPropertyName("times")]
        public TimeOut Times { get; set; }

        [JsonPropertyName("isConnection")]
        public bool IsConnection { get; set; }

        public RoomSettingsDto(RoomType type, string roomId,
            bool isConnection, TimeOut times)
        {
            Type = type;
            RoomId = roomId;
            IsConnection = isConnection;
            Times = times;
        }
    }
}
