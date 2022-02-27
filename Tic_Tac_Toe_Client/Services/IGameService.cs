using TicTacToe.Client.Enums;

namespace TicTacToe.Client.Services;

public interface IGameService
{
    Task<HttpResponseMessage> StartRoomAsync(RoomType roomType, string roomId, bool isConnect);

    Task<HttpResponseMessage> CheckSecondPlayerAsync();

    Task<HttpResponseMessage> MakeMoveAsync(int index, int number);

    Task<HttpResponseMessage> CheckMoveAsync();

}
