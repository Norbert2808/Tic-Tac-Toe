using TicTacToe.Client.Enums;

namespace TicTacToe.Client.Services;

public interface IGameService
{
    Task<HttpResponseMessage> StartSessionAsync(RoomType roomType, string roomId);
}
