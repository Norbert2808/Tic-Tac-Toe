using Tic_Tac_Toe.Client.Enums;

namespace Tic_Tac_Toe.Client.Services;

public interface IGameService
{
    Task<HttpResponseMessage> StartSessionAsync(RoomType roomType, string roomId);
}
