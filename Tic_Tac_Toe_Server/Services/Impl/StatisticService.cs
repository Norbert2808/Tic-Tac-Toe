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

        private List<Move> GetAllMovesFromRoomStorage(string login)
        {
            var result = new List<Move>();
            _roomStorage.ForEach(x =>
            {
                if (login.Equals(x.LoginFirstPlayer, StringComparison.Ordinal))
                {
                    foreach (var moves in x.Rounds)
                    {
                        result.AddRange(moves.FirstPlayerMove);
                    }
                }
                else if (login.Equals(x.LoginSecondPlayer, StringComparison.Ordinal))
                {
                    foreach (var moves in x.Rounds)
                    {
                        result.AddRange(moves.FirstPlayerMove);
                    }
                }
            });

            return result;
        }

        private (int, int) GetWinLostCount(string login)
        {
            var winCount = 0;
            var lostCount = 0;
            _roomStorage.ForEach(x =>
            {
                if (login.Equals(x.LoginFirstPlayer, StringComparison.Ordinal))
                {
                    if (x.FirstWin)
                        winCount++;
                    else
                        lostCount++;
                }
                else if (login.Equals(x.LoginSecondPlayer, StringComparison.Ordinal))
                {
                    if (x.FirstWin)
                        lostCount++;
                    else
                        winCount++;
                }
            });

            return (winCount, lostCount);
        }

       //public List<int> GetTopNumbers(List<Move> moves)
       // {
       //     var countOfNumbers = new List<int>(9);
       //     moves.ForEach(x => countOfNumbers[x.Number - 1] += 1);
       //     var topCount = countOfNumbers.Max();

       //     var result = new List<int>();
       //     var index = 0;
       //     countOfNumbers.ForEach(x =>
       //     {
       //         if (x == topCount)
       //             result.Add(index + 1);
       //         index++;
       //     });
       //     return result;
       // } 

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
