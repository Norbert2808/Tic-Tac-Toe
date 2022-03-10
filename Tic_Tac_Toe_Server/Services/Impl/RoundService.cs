using TicTacToe.Server.DTO;
using TicTacToe.Server.Exceptions;
using TicTacToe.Server.Models;

namespace TicTacToe.Server.Services.Impl
{
    public class RoundService : IRoundService
    {
        public RoundStateDto? CheckMove(Room room, bool isFirst)
        {
            var round = room.Rounds.Peek();
            var isOpponentMove = round.CheckOpponentsMove(isFirst);

            if (room.Times.IsRoundTimeOut())
            {
                room.ConfirmFirstPlayer = false;
                room.ConfirmSecondPlayer = false;

                if (isFirst)
                    room.FirstPlayer.Wins++;
                else
                    room.SecondPlayer.Wins++;

                round.IsFinished = true;
                throw new TimeOutException("Time out,  your opponent didn't moved.");
            }

            if (isOpponentMove)
            {
                if (round.CheckEndOfGame())
                {
                    room.ConfirmFirstPlayer = false;
                    room.ConfirmSecondPlayer = false;
                    round.IsFinished = true;
                    if (isFirst)
                    {
                        room.SecondPlayer.Wins++;
                    }
                    else
                    {
                        room.FirstPlayer.Wins++;
                    }
                }

                return new RoundStateDto
                {
                    Board = round.Board,
                    IsFinished = round.IsFinished,
                    IsFirstPlayer = isFirst,
                    IsActiveFirstPlayer = round.IsActiveFirstPlayer
                };
            }

            return round.IsFinished ? throw new GameException("Your opponent surrender.") : null;
        }

        public RoundStateDto CheckState(Room room, bool isFirst)
        {
            var round = room.Rounds.Peek();

            return new RoundStateDto
            {
                Board = round.Board,
                IsFinished = round.IsFinished,
                IsFirstPlayer = isFirst,
                IsActiveFirstPlayer = round.IsActiveFirstPlayer,
            };
        }

        public void DoMove(Room room, MoveDto move, bool isFirst)
        {
            var round = room.Rounds.Peek();

            var isValid = round.DoMove(move, isFirst);
            if (isValid)
            {
                room.Times.LastMoveTime = DateTime.UtcNow;
                round.IsActiveFirstPlayer = !round.IsActiveFirstPlayer;
                if (round.CheckEndOfGame())
                {
                    room.ConfirmFirstPlayer = false;
                    room.ConfirmSecondPlayer = false;
                    round.IsFinished = true;
                }
            }
        }

        public void ExitFormRound(Room room, bool isFirst)
        {
            var round = room.Rounds.Peek();

            round.IsFinished = true;

            room.ConfirmFirstPlayer = false;
            room.ConfirmSecondPlayer = false;

            if (isFirst)
                room.SecondPlayer.Wins++;
            else
                room.FirstPlayer.Wins++;
        }
    }
}
