using TicTacToe.Client.DTO;
using TicTacToe.Client.Enums;

namespace TicTacToe.Client.Services;

public interface IGameService
{
    Task<HttpResponseMessage> StartRoomAsync(RoomType roomType, string roomId, bool isConnect);

    Task<HttpResponseMessage> CheckRoomAsync();

    Task<HttpResponseMessage> MakeMoveAsync(MoveDto move);

    Task<HttpResponseMessage> CheckMoveAsync();

    Task<HttpResponseMessage> SendConfirmationAsync();

    Task<HttpResponseMessage> CheckConfirmationAsync();

    Task<bool> CheckPlayerPosition();

    Task<HttpResponseMessage> ExitFromRoomAsync();

}
