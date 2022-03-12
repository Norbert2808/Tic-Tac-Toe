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
        public async Task<IActionResult> GetPrivateStatistic()
        {
            if (LoginUser is null or "")
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            var statistic = await _statisticService.GetPrivateStatistic(LoginUser);
            return Ok(statistic);
        }

        [HttpGet("leaders/{type}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetLeadersStatistic([FromRoute] SortingType type)
        {
            var result = await _statisticService.GetLeaders(type);
            return Ok(result);
        }

    }
}
