using System.Text.Json.Serialization;
using TicTacToe.Server.DTO;
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
        public TimeLimit Times { get; set; }

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
            Times = new TimeLimit
            {
                CreationRoomDate = DateTime.UtcNow,
                RoundTimeOut = settings.TimeOut.RoundTimeOut,
                ConnectionTimeOut = settings.TimeOut.ConnectionTimeOut,
                StartGameTimeOut = settings.TimeOut.StartGameTimeOut,
                RoomActionTimeOut = settings.TimeOut.RoomActionTimeOut
            };
            IsCompleted = false;
            IsFinished = false;
            Rounds = new Stack<Round>();
        }

        [JsonConstructor]
        public Room(string roomId,
            Stack<Round> rounds,
            Player firstPlayer,
            Player secondPlayer,
            TimeLimit times)
        {
            RoomId = roomId;
            Times = times;
            FirstPlayer = firstPlayer;
            SecondPlayer = secondPlayer;
            Rounds = rounds;
        }
    }
}
