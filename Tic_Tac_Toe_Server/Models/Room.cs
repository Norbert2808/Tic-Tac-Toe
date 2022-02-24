using System.Text.Json.Serialization;
using TicTacToe.Server.Enum;

namespace TicTacToe.Server.Models;

public class Room
{
    [JsonPropertyName("RoomId")] 
    public string RoomId { get; set; }

    public TimeSpan ConnectionTimeOut { get; set; } = TimeSpan.FromMinutes(5);

    public TimeSpan RoundTimeOut { get; set; } = TimeSpan.FromMinutes(2);

    [JsonPropertyName("CreationDate")]
    public DateTime CreationDate { get; set; }
    
    public RoomSettings Settings { get; set; }

    [JsonPropertyName("First player")]
    public string LoginFirstPlayer { get; set; }

    [JsonPropertyName("Second player")]
    public string LoginSecondPlayer { get; set; }

    public bool IsCompleted { get; set; }

    public Room(string login, RoomSettings settings)
    {
        LoginFirstPlayer = login;
        Settings = settings;
        RoomId = settings.RoomId.Length == 0 ? Guid.NewGuid().ToString() : settings.RoomId;
        CreationDate = DateTime.UtcNow;
        IsCompleted = false;
    }
}
