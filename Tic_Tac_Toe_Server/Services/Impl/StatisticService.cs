using TicTacToe.Server.Models;
using TicTacToe.Server.Tools;

namespace TicTacToe.Server.Services.Impl
{
    public class StatisticService : IStatisticService
    {
        private const string Path = "roomInfo.json";

        private List<Room> _roomStorage;

        private readonly JsonHelper<Room> _jsonHelper;

        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        public StatisticService()
        {
            _roomStorage = new List<Room>();
            _jsonHelper = new JsonHelper<Room>(Path);
        }

        public async Task<PrivateStatistic> GetPrivateStatistic(string login)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                await UpdateRoomStorageFromFileAsync();

                var winLostCount = GetWinLostCount(login);

                var allMoves = GetAllMovesFromRoomStorage(login);
                var allPosition = allMoves.Select(x => x.IndexOfCell + 1).ToList();
                var allNumbers = allMoves.Select(x => x.Number).ToList();

                var topPosition = GetTopProperty(allPosition);
                var topNumbers = GetTopProperty(allNumbers);

                return new PrivateStatistic(
                    winLostCount.Item1,
                    winLostCount.Item2,
                    topNumbers,
                    topPosition);
            }
            finally
            {
                _ = _semaphoreSlim.Release();
            }
        }
        private async Task UpdateRoomStorageFromFileAsync()
        {
            _roomStorage = await _jsonHelper.DeserializeAsync();
        }

        private List<MoveDto> GetAllMovesFromRoomStorage(string login)
        {
            var result = new List<MoveDto>();
            _roomStorage.ForEach(room =>
            {
                if (login.Equals(room.LoginFirstPlayer, StringComparison.Ordinal))
                {
                    foreach (var moves in room.Rounds)
                    {
                        result.AddRange(moves.FirstPlayerMove);
                    }
                }
                else if (login.Equals(room.LoginSecondPlayer, StringComparison.Ordinal))
                {
                    foreach (var moves in room.Rounds)
                    {
                        result.AddRange(moves.SecondPlayerMove);
                    }
                }
            });

            return result;
        }

        private (int, int) GetWinLostCount(string login)
        {
            var winCount = 0;
            var lostCount = 0;
            _roomStorage.ForEach(room =>
            {
                if (login.Equals(room.LoginFirstPlayer, StringComparison.Ordinal))
                {
                    foreach (var moves in room.Rounds)
                    {
                        //if (moves.FirstWin)
                        //    winCount++;
                        //else
                        //    lostCount++;
                    }
                }
                else if (login.Equals(room.LoginSecondPlayer, StringComparison.Ordinal))
                {
                    foreach (var moves in room.Rounds)
                    {
                        //if (moves.FirstWin)
                        //    lostCount++;
                        //else
                        //    winCount++;
                    }
                }
            });

            return (winCount, lostCount);
        }

        private List<int> GetTopProperty(List<int> prop)
        {
            var countOfPosition = Enumerable.Repeat(0, 9).ToList();
            prop.ForEach(x => countOfPosition[x - 1] += 1);
            var topCount = countOfPosition.Max();

            if (topCount == 0)
                return new List<int>();

            var result = new List<int>();
            var index = 0;
            countOfPosition.ForEach(x =>
            {
                if (x == topCount)
                    result.Add(index + 1);
                index++;
            });
            return result;
        }
    }
}
