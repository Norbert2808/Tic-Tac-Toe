using TicTacToe.Server.DTO;
using TicTacToe.Server.Models;

namespace TicTacToe.Server.Services
{
    public interface IRoundService
    {
        RoundStateDto? CheckMove(Room room, bool isFirst);

        void DoMove(Room room, MoveDto move, bool isFirst);

        RoundStateDto CheckState(Room room, bool isFirst);

        void ExitFormRound(Room room, bool isFirst);
    }
}
