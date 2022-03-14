using System.Text.Json.Serialization;

namespace TicTacToe.Client.DTO
{
    public sealed class TimeIntervalDto
    {
        [JsonPropertyName("startTime")]
        public DateTime StartTime { get; set; }

        [JsonPropertyName("endTime")]
        public DateTime EndTime { get; set; }

        public TimeIntervalDto(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

    }
}
