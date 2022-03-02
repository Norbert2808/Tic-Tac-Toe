using System.Text.Json.Serialization;
using TicTacToe.Server.Enum;

namespace TicTacToe.Server.Models;

public class Room
{
    [JsonPropertyName("RoomId")] 
    public string RoomId { get; set; }

    public TimeSpan ConnectionTimeOut { get; set; } = TimeSpan.FromMinutes(3);

    public TimeSpan StartGameTimeOut { get; set; } = TimeSpan.FromMinutes(2);
    
    public DateTime ConfirmationTime { get; set; }

    public TimeSpan RoundTimeOut { get; set; } = TimeSpan.FromSeconds(20);

    [JsonPropertyName("CreationDate")]
    public DateTime CreationRoomDate { get; set; }
    
    public RoomSettings Settings { get; set; }

    [JsonPropertyName("First player")]
    public string LoginFirstPlayer { get; set; }

    [JsonPropertyName("Second player")]
    public string LoginSecondPlayer { get; set; }

    public bool IsCompleted { get; set; }

    public bool IsBot { get; set; }

    public bool ConfirmFirstPlayer { get; set; }
    
    public bool ConfirmSecondPlayer { get; set; }

    public Room(string login, RoomSettings settings)
    {
        LoginFirstPlayer = login;
        Settings = settings;
        RoomId = settings.RoomId.Length == 0 ? Guid.NewGuid().ToString() : settings.RoomId;
        IsBot = settings.Type == RoomType.Practice;
        CreationRoomDate = DateTime.UtcNow;
        IsCompleted = false;
    }
    
    public TimeSpan GetStartGameWaitingTime() => DateTime.UtcNow - ConfirmationTime;

    public TimeSpan GetConnectionTime() => DateTime.UtcNow - CreationRoomDate;

    public bool IsStartGameTimeOut() => GetStartGameWaitingTime() > StartGameTimeOut;

    public bool IsConnectionTimeOut() => GetConnectionTime() > ConnectionTimeOut;
}
