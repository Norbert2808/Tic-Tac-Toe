using System.Net;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Server.DTO;
using TicTacToe.Server.Enums;
using TicTacToe.Server.Services;

namespace TicTacToe.Server.Controllers
{
    [Route("api/statistic")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private readonly IStatisticService _statisticService;

        private readonly ILogger<StatisticController> _logger;

        public StatisticController(IStatisticService statisticService,
            ILogger<StatisticController> logger)
        {
            _statisticService = statisticService;
            _logger = logger;
        }

        [FromHeader(Name = "Login")]
        public string? LoginUser { get; set; }

        [HttpGet("private")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetPrivateStatisticAsync()
        {
            LogInformationAboutClass(nameof(GetPrivateStatisticAsync), $"Processing request: {Request.Path}");
            if (string.IsNullOrEmpty(LoginUser))
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            LogInformationAboutClass(nameof(GetPrivateStatisticAsync),
                $"Invoke method {nameof(_statisticService.GetPrivateStatisticAsync)}");

            var statistic = await _statisticService.GetPrivateStatisticAsync(LoginUser,
                DateTime.MinValue, DateTime.MaxValue);
            return Ok(statistic);
        }

        [HttpPost("private-time-interval")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetPrivateStatisticInTimeIntervalAsync(
            [FromBody] TimeIntervalDto statisticTime)
        {
            LogInformationAboutClass(nameof(GetPrivateStatisticInTimeIntervalAsync),
                $"Processing request: {Request.Path}");
            if (string.IsNullOrEmpty(LoginUser))
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            LogInformationAboutClass(nameof(GetPrivateStatisticInTimeIntervalAsync),
                $"Invoke method {nameof(_statisticService.GetPrivateStatisticAsync)}");

            var statistic = await _statisticService.GetPrivateStatisticAsync(
                LoginUser, statisticTime.StartTime, statisticTime.EndTime);
            return Ok(statistic);
        }

        [HttpGet("leaders/{type}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetLeadersStatisticAsync([FromRoute] SortingType type)
        {
            LogInformationAboutClass(nameof(GetLeadersStatisticAsync),
                $"Processing request: {Request.Path}." +
                $" Invoke method {nameof(_statisticService.GetLeadersAsync)}");

            var result = await _statisticService.GetLeadersAsync(type);
            return Ok(result);
        }

        [NonAction]
        private void LogInformationAboutClass(string methodName, string message)
        {
            _logger.LogInformation("{ClassName}::{MethodName}::{Message}",
                nameof(StatisticController), methodName, message);
        }
    }
}
