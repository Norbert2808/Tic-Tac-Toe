using TicTacToe.Client.Enums;

namespace TicTacToe.Client.DTO;

public class SessionSettings
{
    public RoomType Type { get; set; }

    public string RoomId { get; set; }

    public SessionSettings(RoomType type, string roomId)
    {
        Type = type;
        RoomId = roomId;
    }
}
