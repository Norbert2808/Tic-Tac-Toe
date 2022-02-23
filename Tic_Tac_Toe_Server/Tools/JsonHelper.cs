using System.Text.Json;

namespace TicTacToe.Server.Tools
{
    public sealed class JsonHelper<T>
    {
        private readonly string _path;

        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public JsonHelper(string path)
        {
            _path = path;
        }

        public async Task<List<T>> DeserializeAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!File.Exists(_path))
                {
                    File.Create(_path).Close();
                }

                using var fs = File.OpenRead(_path);
                return await JsonSerializer.DeserializeAsync<List<T>>(fs) ?? new List<T>();
            }
            finally
            {
                _ = _semaphore.Release();
            }
        }

        public async Task SerializeAsync(List<T> data)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!File.Exists(_path))
                {
                    File.Create(_path).Close();
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var jsonString = JsonSerializer.Serialize(data, options);
                await File.WriteAllTextAsync(_path, jsonString);
            }
            finally
            {
                _ = _semaphore.Release();
            }
        }
    }
}
