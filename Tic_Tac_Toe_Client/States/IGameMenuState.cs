using TicTacToe.Client.Enums;

namespace TicTacToe.Client.States;

public interface IGameMenuState : IState
{
    Task StartConnectionWithRoomAsync(RoomType type, string roomId);

    Task WaitSecondPlayerAsync();
}
