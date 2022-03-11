using TicTacToe.Server.DTO;
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

        public async Task<PrivateStatisticDto> GetPrivateStatistic(string login)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                await UpdateRoomStorageFromFileAsync();

                var winLostCount = GetWinLostCount(login);

                var allMoves = GetAllMovesFromRoomStorage(login);
                (var allTime, var allRoomsCount) = GetAllTimeAndCountsOfRooms(login);
                var allMovesCount = allMoves.Count;
                var allPosition = allMoves.Select(x => x.IndexOfCell + 1).ToList();
                var allNumbers = allMoves.Select(x => x.Number).ToList();

                (var topPosition, var countOfPositionUse) = GetTopPropertyWithCount(allPosition);
                (var topNumbers, var countOfNumbersUse) = GetTopPropertyWithCount(allNumbers);

                return new PrivateStatisticDto(
                    winLostCount.Item1,
                    winLostCount.Item2,
                    allRoomsCount,
                    allMovesCount,
                    topNumbers,
                    countOfNumbersUse,
                    topPosition,
                    countOfPositionUse,
                    allTime);
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
                if (login.Equals(room.FirstPlayer.Login, StringComparison.Ordinal))
                {
                    foreach (var moves in room.Rounds)
                    {
                        result.AddRange(moves.FirstPlayerMove);
                    }
                }
                else if (login.Equals(room.SecondPlayer.Login, StringComparison.Ordinal))
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
                if (login.Equals(room.FirstPlayer.Login, StringComparison.Ordinal))
                {
                    winCount += room.FirstPlayer.Wins;
                    lostCount += room.SecondPlayer.Wins;
                }
                else if (login.Equals(room.SecondPlayer.Login, StringComparison.Ordinal))
                {
                    winCount += room.SecondPlayer.Wins;
                    lostCount += room.FirstPlayer.Wins;
                }
            });

            return (winCount, lostCount);
        }

        private (List<int>, int) GetTopPropertyWithCount(List<int> prop)
        {
            var countOfPosition = Enumerable.Repeat(0, 9).ToList();
            prop.ForEach(x => countOfPosition[x - 1] += 1);
            var topCount = countOfPosition.Max();

            if (topCount == 0)
                return (new List<int>(), 0);

            var result = new List<int>();
            var index = 0;
            countOfPosition.ForEach(x =>
            {
                if (x == topCount)
                    result.Add(index + 1);
                index++;
            });
            return (result, topCount);
        }

        private (TimeSpan, int) GetAllTimeAndCountsOfRooms(string login)
        {
            var allTime = TimeSpan.Zero;
            var countOfRooms = 0;
            _roomStorage.ForEach(room =>
            {
                if (login.Equals(room.FirstPlayer.Login, StringComparison.Ordinal)
                    || login.Equals(room.SecondPlayer.Login, StringComparison.Ordinal))
                {
                    allTime += room.Times.FinishRoomDate - room.Times.CreationRoomDate;
                    countOfRooms++;
                }
            });
            return (allTime, countOfRooms);
        }

        //private int GetCountOfRooms(string login)
        //{
        //    var result = 0;
        //    _roomStorage.ForEach(room =>
        //    {
        //        if (login.Equals(room.FirstPlayer.Login, StringComparison.Ordinal)
        //            || login.Equals(room.SecondPlayer.Login, StringComparison.Ordinal))
        //        {
        //            result++;
        //        }
        //    });

        //    return result;
        //}
    }
}
