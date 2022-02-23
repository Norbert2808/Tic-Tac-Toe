using Tic_Tac_Toe.Client.Enums;

namespace Tic_Tac_Toe.Client.DTO;

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
