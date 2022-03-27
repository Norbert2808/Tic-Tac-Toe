namespace TicTacToe.Server.Tools;

public interface IJsonHelper<T>
{
    Task<List<T>> DeserializeAsync();

    Task AddObjectToFileAsync(T data);
}
