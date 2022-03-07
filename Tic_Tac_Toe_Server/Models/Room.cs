using System.Text.Json.Serialization;
using TicTacToe.Server.Enums;

namespace TicTacToe.Server.Models
{
    public class Room
    {
        [JsonPropertyName("roomId")]
        public string RoomId { get; set; }

        //Times
        [JsonIgnore]
        public TimeSpan ConnectionTimeOut { get; set; } = TimeSpan.FromMinutes(3);

        [JsonIgnore]
        public TimeSpan StartGameTimeOut { get; set; } = TimeSpan.FromMinutes(2);

        [JsonIgnore]
        public DateTime ConfirmationTime { get; set; }

        [JsonIgnore]
        public TimeSpan RoundTimeOut { get; set; } = TimeSpan.FromSeconds(20);

        [JsonIgnore]
        public DateTime LastMoveTime { get; set; }

        [JsonPropertyName("creationDate")]
        public DateTime CreationRoomDate { get; set; }

        [JsonPropertyName("finishDate")]
        public DateTime FinishRoomDate { get; set; }

        public RoomSettings Settings { get; set; }

        [JsonPropertyName("firstPlayer")]
        public string LoginFirstPlayer { get; set; }

        [JsonPropertyName("secondPlayer")]
        public string LoginSecondPlayer { get; set; }

        //Flags 
        [JsonIgnore]
        public bool IsFinished { get; set; }

        [JsonIgnore]
        public bool IsCompleted { get; set; }

        [JsonIgnore]
        public bool IsBot { get; set; }

        [JsonIgnore]
        public bool ConfirmFirstPlayer { get; set; }

        [JsonIgnore]
        public bool ConfirmSecondPlayer { get; set; }

        [JsonPropertyName("firstWin")]
        public bool FirstWin { get; set; }

        [JsonPropertyName("rounds")]
        public Stack<Round> Rounds { get; private set; }

        public Room(string login, RoomSettings settings)
        {
            LoginFirstPlayer = login;
            LoginSecondPlayer = "";
            Settings = settings;
            RoomId = settings.RoomId.Length == 0 ? Guid.NewGuid().ToString() : settings.RoomId;
            IsBot = settings.Type == RoomType.Practice;
            CreationRoomDate = DateTime.UtcNow;
            IsCompleted = false;
            IsFinished = false;
            Rounds = new Stack<Round>();
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
    }
}
