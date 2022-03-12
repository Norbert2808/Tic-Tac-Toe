using System.Text.Json.Serialization;

namespace TicTacToe.Server.DTO
{
    public sealed class LeaderStatisticDto
    {
        [JsonPropertyName("login")]
        public string Login { get; set; }

        [JsonPropertyName("winnings")]
        public int Winnings { get; set; }

        [JsonPropertyName("losses")]
        public int Losses { get; set; }

        [JsonPropertyName("winRate")]
        public double WinRate { get; set; }

        [JsonPropertyName("roomsNumber")]
        public int RoomsNumber { get; set; }

        [JsonPropertyName("time")]
        public TimeSpan Time { get; set; }

        public LeaderStatisticDto(string login, int winnings,
            int losses, int roomsNumber, TimeSpan time)
        {
            Login = login;
            Winnings = winnings;
            Losses = losses;
            WinRate = Math.Round((double)winnings / (winnings + losses) * 100);
            RoomsNumber = roomsNumber;
            Time = time;
        }
    }
}
