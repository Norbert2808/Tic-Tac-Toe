using System.Text.Json;
using TicTacToe.Client.DTO;
using TicTacToe.Client.Enums;
using TicTacToe.Client.Models;

namespace TicTacToe.Client.Services.Impl
{
    public class SettingsService : ISettingsService
    {
        private const string SettingsFileName = "timeSettings.json";

        private List<TimeOut?> _timeOutStorage = new();

        private readonly JsonSerializerOptions _options;

        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public SettingsService()
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            CheckFile();
        }

        public async Task<RoomSettingsDto> ConfigureSettingsAsync(string login, RoomType type,
            string roomId, bool isConnecting)
        {
            _ = _semaphore.WaitAsync();
            TimeOut? userSettings;
            try
            {
                userSettings = await FindSettingsByLogin(login);
            }
            catch (ArgumentException)
            {
                userSettings = null;
            }
            finally
            {
                _ = _semaphore.Release();
            }

            return userSettings is null
                   ? new RoomSettingsDto(type, roomId, isConnecting, new TimeOut())
                   : new RoomSettingsDto(type, roomId, isConnecting, userSettings);
        }

        private async Task<List<TimeOut?>> DeserializeAsync()
        {
            await using var fs = File.OpenRead(SettingsFileName);

            if (fs.Length == 0)
                return new List<TimeOut?>();

            var timeOuts = await JsonSerializer.DeserializeAsync<List<TimeOut?>>(fs);

            timeOuts ??= new List<TimeOut?>();
            return timeOuts;
        }

        private async Task<TimeOut?> FindSettingsByLogin(string login)
        {
            _timeOutStorage = await DeserializeAsync();

            if (_timeOutStorage.Count == 0)
                return null;

            var userSettings = _timeOutStorage
                .LastOrDefault(x =>
                {
                    _ = x!.LoginSettingsOwner ?? throw new ArgumentException(nameof(x.LoginSettingsOwner));
                    return x.LoginSettingsOwner.Equals(login, StringComparison.Ordinal);
                });

            return userSettings;
        }

        public async Task SerializeAsync(TimeOut timeOut)
        {
            _ = timeOut ?? throw new ArgumentNullException(nameof(timeOut));
            _ = timeOut.LoginSettingsOwner ?? throw new ArgumentException(nameof(timeOut.LoginSettingsOwner));

            _ = _semaphore.WaitAsync();
            try
            {
                var activeSettings = await FindSettingsByLogin(timeOut.LoginSettingsOwner);
                if (activeSettings is not null)
                    _ = _timeOutStorage.Remove(activeSettings);

                _timeOutStorage.Add(timeOut);
                var jsonString = JsonSerializer.Serialize(_timeOutStorage, _options);
                await File.WriteAllTextAsync(SettingsFileName, jsonString);
            }
            finally
            {
                _ = _semaphore.Release();
            }
        }

        private void CheckFile()
        {
            if (!File.Exists(SettingsFileName))
                _ = File.Create(SettingsFileName);
        }
    }
}
