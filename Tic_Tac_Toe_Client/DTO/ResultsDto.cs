

namespace TicTacToe.Client.DTO
{
    public sealed class ResultsDto
    {
        public string? LoginFirstPlayer { get; set; }

        public string? LoginSecondPlayer { get; set; }

        public int WinFirst { get; set; }

        public int WinSecond { get; set; }
    }
}
