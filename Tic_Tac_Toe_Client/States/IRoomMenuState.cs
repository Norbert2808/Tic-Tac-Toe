using TicTacToe.Client.Enums;

namespace TicTacToe.Client.States
{
    public interface IRoomMenuState : IState
    {
        Task StartConnectionWithRoomAsync(RoomType type, string roomId, bool isConnecting);

        Task WaitSecondPlayerAsync(string[] message);

    }
}
