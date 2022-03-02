using TicTacToe.Client.Enums;

namespace TicTacToe.Client.States;

public interface IGameMenuState : IState
{
    Task StartConnectionWithRoomAsync(RoomType type, string roomId, bool isConnecting);

    Task WaitSecondPlayerAsync(string[] message);
    
    Task<string> GetMessageFromResponseAsync(HttpResponseMessage response);
}
