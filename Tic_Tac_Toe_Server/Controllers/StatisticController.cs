﻿using System.Net;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetPrivateStatistiAsync()
        {
            if (LoginUser is null or "")
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }
            _logger.LogInformation("StatisticController::Invoke method :: GetPrivateStatisticAsync");
            var statistic = await _statisticService.GetPrivateStatisticAsync(LoginUser);
            return Ok(statistic);
        }

        [HttpGet("leaders/{type}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetLeadersStatisticAsync([FromRoute] SortingType type)
        {
            _logger.LogInformation("StatisticController::Invoke method :: GetLeadersStatisticAsync");
            var result = await _statisticService.GetLeadersAsync(type);
            return Ok(result);
        }

    }
}
