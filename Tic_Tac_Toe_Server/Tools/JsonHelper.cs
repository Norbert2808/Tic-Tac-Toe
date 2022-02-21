using System.Collections.Concurrent;
using System.Text.Json;

namespace Tic_Tac_Toe.Server.Tools
{
    public sealed class JsonHelper<T>
    {
        private readonly string _path;

        public JsonHelper(string path)
        {
            _path = path;
        }

        public async Task<List<T>> DeserializeAsync()
        {
            if (!File.Exists(_path))
            {
                File.Create(_path).Close();
            }

            using var fs = File.OpenRead(_path);
            return await JsonSerializer.DeserializeAsync<List<T>>(fs) ?? new List<T>();
        }
    }
}
