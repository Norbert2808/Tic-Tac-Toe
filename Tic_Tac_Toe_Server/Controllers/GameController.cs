using System.Net;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Server.Models;
using TicTacToe.Server.Services;

namespace TicTacToe.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly IRoomService _roomService;

    private readonly IAccountService _accountService;

    private readonly ILogger<GameController> _logger;

    public GameController(IRoomService roomService,
        IAccountService accountService,
        ILogger<GameController> logger)
    {
        _roomService = roomService;
        _accountService = accountService;
        _logger = logger;
    }

    [FromHeader(Name = "Login")]
    public string LoginUser { get; set; }

    [HttpPost("create_room")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> StartSessionAsync([FromBody] RoomSettings? settings)
    {
        if (!await FindUser())
        {
            _logger.LogWarning("Unauthorized users");
            return Unauthorized("Unauthorized users");
        }

        if (settings is null)
        {
            _logger.LogWarning("Settings is null");
            return BadRequest("Settings is null");
        }

        var response = await _roomService.CreateRoomAsync(LoginUser, settings);

        return Ok(response);
    }

    [NonAction]
    public async Task<bool> FindUser()
    {
        return await _accountService.FindAccountByLogin(LoginUser);
    }
    
}
