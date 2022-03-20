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

            if (room.IsBot)
            {
                round = room.Rounds.Peek();
                DoMove(room, round.GetValidMoveFromBot(), false);
            }

            var isOpponentMove = round.CheckOpponentsMove(isFirst);

            if (room.Times.IsRoundTimeOut())
            {
                SetDefaultsSettingForRoom(room);

                if (isFirst)
                    room.FirstPlayer.Wins++;
                else
                    room.SecondPlayer.Wins++;

                throw new TimeoutException("Time out,  your opponent didn't moved.");
            }

            if (isOpponentMove)
            {
                if (round.CheckEndOfGame())
                {
                    SetDefaultsSettingForRoom(room);

                    if (isFirst)
                        room.SecondPlayer.Wins++;
                    else
                        room.FirstPlayer.Wins++;
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

        public void CreateNewRound(Room room)
        {
            room.Rounds.Push(new Round());
            room.Times.LastMoveTime = DateTime.UtcNow;
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
                    if (room.IsBot)
                        room.FirstPlayer.Wins++;

                    SetDefaultsSettingForRoom(room);
                }
            }
        }

        public void ExitFormRound(Room room, bool isFirst)
        {
            var round = room.Rounds.Peek();

            if (!round.IsFinished)
            {
                SetDefaultsSettingForRoom(room);

                if (isFirst)
                    room.SecondPlayer.Wins++;
                else
                    room.FirstPlayer.Wins++;
            }
        }

        private void SetDefaultsSettingForRoom(Room room)
        {
            var round = room.Rounds.Peek();

            room.Times.ActionTimeInRoom = DateTime.UtcNow;
            room.ConfirmFirstPlayer = false;
            room.ConfirmSecondPlayer = false;
            round.IsFinished = true;
        }
    }
}
