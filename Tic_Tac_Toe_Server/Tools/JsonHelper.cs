using System.Text;
using System.Text.Json;

namespace TicTacToe.Server.Tools
{
    public sealed class JsonHelper<T>
    {
        private readonly string _path;

        private readonly SemaphoreSlim _semaphore = new(1, 1);

        private readonly JsonSerializerOptions _options;

        public JsonHelper(string path)
        {
            _path = path;
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
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
            catch
            {
                return await Task.FromResult(new List<T>());
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

                var jsonString = JsonSerializer.Serialize(data, _options);
                await File.WriteAllTextAsync(_path, jsonString);
            }
            finally
            {
                _ = _semaphore.Release();
            }
        }

        public string Serialize(T data)
        {
            if (!File.Exists(_path))
            {
                File.Create(_path).Close();
            }

            return JsonSerializer.Serialize(data, _options);
        }

        public async Task AddAccountToFile(T data)
        {
            await _semaphore.WaitAsync();
            try
            {
                using var fs = new FileStream(_path, FileMode.Open);
                _ = fs.Seek(-3, SeekOrigin.End);

                var jsonObj = Serialize(data);
                var insertStr = $",\n  {jsonObj.Replace("\r\n", "\n  ")} \n]";
                var insertStrBytes = Encoding.UTF8.GetBytes(insertStr);

                await fs.WriteAsync(insertStrBytes);
            }
            finally
            {
                _ = _semaphore.Release();
            }
        }
    }
}
