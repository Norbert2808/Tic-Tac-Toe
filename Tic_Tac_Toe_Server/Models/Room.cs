using System.Text.Json.Serialization;
using TicTacToe.Server.Enums;

namespace TicTacToe.Server.Models
{
    public class Room
    {
        [JsonPropertyName("roomId")]
        public string RoomId { get; set; }

        [JsonPropertyName("rounds")]
        public Stack<Round> Rounds { get; private set; }

        [JsonIgnore]
        public RoomSettingsDto Settings { get; set; }

        [JsonPropertyName("firstPlayer")]
        public Player FirstPlayer { get; set; }

        [JsonPropertyName("secondPlayer")]
        public Player SecondPlayer { get; set; }

        [JsonPropertyName("times")]
        public TimeLimit TimeOuts { get; set; }

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

        public Room(string login, RoomSettingsDto settings)
        {
            FirstPlayer = new Player(login, 0);
            SecondPlayer = new Player("", 0);
            Settings = settings;
            RoomId = settings.RoomId.Length == 0 ? Guid.NewGuid().ToString() : settings.RoomId;
            IsBot = settings.Type == RoomType.Practice;
            TimeOuts = new TimeLimit
            {
                CreationRoomDate = DateTime.UtcNow
            };
            IsCompleted = false;
            IsFinished = false;
            Rounds = new Stack<Round>();
        }

        [JsonConstructor]
        public Room(string roomId,
            DateTime creationRoomDate,
            DateTime finishRoomDate,
            Player firstPlayer,
            Player secondPlayer,
            Stack<Round> rounds)
        {
            RoomId = roomId;
            TimeOuts = new TimeLimit
            {
                CreationRoomDate = creationRoomDate,
                FinishRoomDate = finishRoomDate
            };
            FirstPlayer = firstPlayer;
            SecondPlayer = secondPlayer;
            Rounds = rounds;
        }
    }
}
