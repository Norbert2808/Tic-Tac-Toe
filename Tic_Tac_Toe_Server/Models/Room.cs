using System.Text.Json.Serialization;

namespace Tic_Tac_Toe.Server.Models;

public class Room
{
    [JsonPropertyName("RoomId")]
    public string RoomId { get; set; }

    public TimeSpan ConnectionTimeOut { get; set; } = TimeSpan.FromMinutes(5);

    public TimeSpan RoundTimeOut { get; set; } = TimeSpan.FromMinutes(2);

    [JsonPropertyName("CreationDate")]
    public DateTime CreationDate { get; set; }
    
    public RoomSettings Settings { get; set; }

    public string LoginFirstPlayer { get; set; }

    public string LoginSecondPlayer { get; set; }

    public bool IsClosed { get; set; }

    public Room(string login, RoomSettings settings)
    {
        LoginFirstPlayer = login;
        Settings = settings;
        RoomId = settings.RoomId;
        CreationDate = DateTime.UtcNow;
    }
}
