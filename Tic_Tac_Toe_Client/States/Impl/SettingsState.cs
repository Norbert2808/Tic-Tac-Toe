using System.Globalization;
using Microsoft.Extensions.Logging;
using TicTacToe.Client.Enums;
using TicTacToe.Client.Models;
using TicTacToe.Client.Services;
using TicTacToe.Client.Tools;

namespace TicTacToe.Client.States.Impl
{
    public class SettingsState : ISettingsState
    {
        private readonly ISettingsService _settingService;

        private readonly IUserService _userService;

        private readonly TimeOut _timeOut;

        private readonly ILogger<SettingsState> _logger;

        public SettingsState(ISettingsService settingsService,
            IUserService userService,
            ILogger<SettingsState> logger)
        {
            _logger = logger;
            _settingService = settingsService;
            _userService = userService;
            _timeOut = new TimeOut();
        }

        public async Task InvokeMenuAsync()
        {
            LogInformationAboutClass(nameof(InvokeMenuAsync), "Execute method");

            _timeOut.LoginSettingsOwner = _userService.Login;
            while (true)
            {
                Console.Clear();
                ConsoleHelper.WriteInConsole(new[]
                {
                    "Settings",
                    "Please choose action:",
                    "1 -- Set time out for start game",
                    "2 -- Set time out for connection in room",
                    "3 -- Set time out for action in room",
                    "4 -- Set time out for round",
                    "0 -- Close"
                }, ConsoleColor.Cyan, "");
                ConsoleHelper.WriteInConsole(new[] { "These settings apply only if you create a room!" },
                    ConsoleColor.Red);
                try
                {
                    ConsoleHelper.ReadIntFromConsole(out var choose);

                    LogInformationAboutClass(nameof(InvokeMenuAsync), "Player choose " +
                                                                      (TimeOutType)Enum.Parse(typeof(TimeOutType),
                                                                          choose.ToString()));

                    switch (choose)
                    {
                        case 1:
                            PopTimeMenu(TimeOutType.StartGameTimeOut);
                            break;

                        case 2:
                            PopTimeMenu(TimeOutType.ConnectionTimeOut);
                            break;

                        case 3:
                            PopTimeMenu(TimeOutType.ActionTimeOut);
                            break;

                        case 4:
                            PopTimeMenu(TimeOutType.RoundTimeOut);
                            break;

                        case 0:
                            await CloseMenuAsync();
                            return;

                        default:
                            continue;
                    }
                }
                catch (FormatException ex)
                {
                    _logger.LogError("Exception invalid format::{Message}", ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "It's not a number!" },
                        ConsoleColor.Red);
                    _ = Console.ReadLine();
                }
                catch (OverflowException ex)
                {
                    _logger.LogError("Number is very large: {Message}", ex.Message);
                    ConsoleHelper.WriteInConsole(new[] { "Number is very large!" },
                        ConsoleColor.Red);
                    _ = Console.ReadLine();
                }
            }
        }

        public void LogInformationAboutClass(string methodName, string message)
        {
            _logger.LogInformation("{ClassName}::{MethodName}::{Message}",
                nameof(SettingsState), methodName, message);
        }

        public void PopTimeMenu(TimeOutType timeOutType)
        {
            while (true)
            {
                Console.Clear();
                ConsoleHelper.WriteInConsole(new[]
                {
                    "Choose format:",
                    "1 -- Minutes",
                    "2 -- Seconds"
                }, ConsoleColor.Cyan);

                try
                {
                    ConsoleHelper.ReadIntFromConsole(out var choose);
                    LogInformationAboutClass(nameof(InvokeMenuAsync), "Player choose " +
                                                                      (TimeType)Enum.Parse(typeof(TimeType),
                                                                          choose.ToString()));
                    switch (choose)
                    {
                        case 1:
                            SetValueToTimeOut(timeOutType, TimeType.Minutes);
                            return;

                        case 2:
                            SetValueToTimeOut(timeOutType, TimeType.Seconds);
                            return;
                    }
                }
                catch (FormatException ex)
                {
                    _logger.LogError("Exception invalid format::{Message}", ex.Message);
                    ConsoleHelper.WriteInConsole(new[] {"It's not a number!"},
                        ConsoleColor.Red);
                    _ = Console.ReadLine();
                }
                catch (OverflowException ex)
                {
                    _logger.LogError("Number is very large: {Message}", ex.Message);
                    ConsoleHelper.WriteInConsole(new[] {"Number is very large!"},
                        ConsoleColor.Red);
                    _ = Console.ReadLine();
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    _logger.LogError("The number cannot be less than zero: {Message}", ex.Message);
                    ConsoleHelper.WriteInConsole(new[] {"The number cannot be less than zero!"},
                        ConsoleColor.Red);
                    _ = Console.ReadLine();
                }
            }
        }

        public async Task CloseMenuAsync()
        {
            LogInformationAboutClass(nameof(CloseMenuAsync), "Player closed settings state.");
            ConsoleHelper.WriteInConsole("Go out and confirm the changes? y[yes] n[no]\n",
                ConsoleColor.DarkYellow);
            var str = Console.ReadLine();
            str ??= "";
            if (str.Equals("y", StringComparison.OrdinalIgnoreCase))
                await SaveInFileAsync();
        }

        public void SetValueToTimeOut(TimeOutType timeOutType, TimeType timeType)
        {
            while (true)
            {
                Console.Clear();
                ConsoleHelper.WriteInConsole("Please input data\n", ConsoleColor.DarkYellow);

                var timeSpan = TimeSpan.Zero;
                int time;
                switch (timeType)
                {
                    case TimeType.Minutes:
                        time = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);
                        ValidateTime(time);
                        timeSpan = TimeSpan.FromMinutes(time);
                        break;

                    case TimeType.Seconds:
                        time = int.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);
                        ValidateTime(time);
                        timeSpan = TimeSpan.FromSeconds(time);
                        break;

                    default:
                        break;
                }

                SetTimeOuts(timeOutType, timeSpan);
                return;
            }
        }

        private void SetTimeOuts(TimeOutType timeOutType, TimeSpan timeSpan)
        {
            switch (timeOutType)
            {
                case TimeOutType.RoundTimeOut:
                    _timeOut.RoundTimeOut = timeSpan;
                    break;

                case TimeOutType.StartGameTimeOut:
                    _timeOut.StartGameTimeOut = timeSpan;
                    break;

                case TimeOutType.ActionTimeOut:
                    _timeOut.RoomActionTimeOut = timeSpan;
                    break;

                case TimeOutType.ConnectionTimeOut:
                    _timeOut.ConnectionTimeOut = timeSpan;
                    break;

                default:
                    break;
            }
        }

        private static void ValidateTime(int time)
        {
            if (time < 0)
                throw new ArgumentOutOfRangeException(nameof(time));
        }

        private async Task SaveInFileAsync()
        {
            try
            {
                await _settingService.SerializeAsync(_timeOut);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError("Occured unexpected error: {Message}", ex.Message);
                ConsoleHelper.WriteInConsole(new[] { "Occured unexpected error." },
                    ConsoleColor.Red);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError("Occured unexpected error: {Message}", ex.Message);
                ConsoleHelper.WriteInConsole(new[] { "Occured unexpected error." },
                    ConsoleColor.Red);
            }
        }
    }
}
