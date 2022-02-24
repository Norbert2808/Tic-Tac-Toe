using TicTacToe.Server.Enum;

namespace TicTacToe.Server.Models;

public class RoomSettings
{
    public RoomType Type { get; set; }

    public string RoomId { get; set; }

    public bool IsConnection { get; set; }
}
