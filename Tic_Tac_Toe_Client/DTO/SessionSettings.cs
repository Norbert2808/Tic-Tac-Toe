using TicTacToe.Client.Enums;

namespace TicTacToe.Client.DTO;

public class SessionSettings
{
    public RoomType Type { get; set; }

    public string RoomId { get; set; }

    public bool IsConnection { get; set; }

    public SessionSettings(RoomType type, string roomId, bool isConnection)
    {
        Type = type;
        RoomId = roomId;
        IsConnection = isConnection;
    }
}
