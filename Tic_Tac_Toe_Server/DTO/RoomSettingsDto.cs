using TicTacToe.Server.Enums;

namespace TicTacToe.Server.Models
{
    public class RoomSettingsDto
    {
        public RoomType Type { get; set; }

        public string RoomId { get; set; }

        public bool IsConnection { get; set; }
    }
}
