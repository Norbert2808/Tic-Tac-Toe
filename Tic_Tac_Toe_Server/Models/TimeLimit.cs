using System.Text.Json.Serialization;

namespace TicTacToe.Server.Models
{
    public sealed class TimeLimit
    {
        [JsonIgnore]
        public TimeSpan RoundTimeOut { get; set; } = TimeSpan.FromSeconds(20);

        [JsonIgnore]
        public TimeSpan ConnectionTimeOut { get; set; } = TimeSpan.FromMinutes(3);

        [JsonIgnore]
        public TimeSpan StartGameTimeOut { get; set; } = TimeSpan.FromMinutes(1);

        [JsonIgnore]
        public TimeSpan RoomActionTimeOut { get; set; } = TimeSpan.FromMinutes(2);

        [JsonIgnore]
        public DateTime ConfirmationTime { get; set; }

        [JsonIgnore]
        public DateTime ActionTimeInRoom { get; set; }

        [JsonIgnore]
        public DateTime LastMoveTime { get; set; }

        [JsonPropertyName("creationRoomDate")]
        public DateTime CreationRoomDate { get; set; }

        [JsonPropertyName("finishRoomDate")]
        public DateTime FinishRoomDate { get; set; }

        public TimeLimit()
        {

        }

        [JsonConstructor]
        public TimeLimit(DateTime creationRoomDate,
            DateTime finishRoomDate)
        {
            CreationRoomDate = creationRoomDate;
            FinishRoomDate = finishRoomDate;
        }

        public TimeSpan GetStartGameWaitingTime()
        {
            return DateTime.UtcNow - ConfirmationTime;
        }

        public TimeSpan GetConnectionTime()
        {
            return DateTime.UtcNow - CreationRoomDate;
        }

        public TimeSpan GetRoundTime()
        {
            return DateTime.UtcNow - LastMoveTime;
        }

        public TimeSpan GetActionTime()
        {
            return DateTime.UtcNow - ActionTimeInRoom;
        }

        public bool IsStartGameTimeOut()
        {
            return GetStartGameWaitingTime() > StartGameTimeOut;
        }

        public bool IsConnectionTimeOut()
        {
            return GetConnectionTime() > ConnectionTimeOut;
        }

        public bool IsRoundTimeOut()
        {
            return GetRoundTime() > RoundTimeOut;
        }

        public bool IsRoomActionTimeOut()
        {
            return GetActionTime() > RoomActionTimeOut;
        }
    }
}
