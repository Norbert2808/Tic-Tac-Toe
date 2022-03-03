using System.Net;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Server.Models;
using TicTacToe.Server.Services;

namespace TicTacToe.Server.Controllers
{
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
        public async Task<IActionResult> StartRoomAsync([FromBody] RoomSettings? settings)
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
            return response is null ? BadRequest("Such a token does not exist.") : Ok(response);
        }

        [HttpGet("check_room/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> CheckRoomAsync(string id)
        {
            if (!await FindUser())
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            var room = await _roomService.FindRoomByIdAsync(id);

            if (room is null)
                return BadRequest();

            if (room.IsCompleted)
                return Ok(new[] { room.LoginFirstPlayer, room.LoginSecondPlayer });

            if (!room.IsConnectionTimeOut())
                return NotFound(room.GetConnectionTime().ToString(@"dd\:mm\:ss"));

            _roomService.DeleteRoom(room);
            return Conflict("Time out");
        }

        [HttpGet("check_move/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CheckMoveAsync(string id)
        {
            if (!await FindUser())
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }


            return NotFound();
        }

        [HttpGet("move/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CheckMoveAsync(string id, [FromBody] Move move)
        {
            if (!await FindUser())
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            return NotFound();
        }

        [HttpPost("send_confirmation/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> AppendConfirmationAsync([FromBody] bool confirmation, string id)
        {
            if (!await FindUser())
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            var room = await _roomService.AppendConfirmation(confirmation, id, LoginUser);

            if (room is null)
                return NotFound("Room is not found.");

            return room.IsStartGameTimeOut() ? Conflict("Time out") : Ok();
        }

        [HttpGet("check_confirmation/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> CheckConfirmationAsync(string id)
        {
            if (!await FindUser())
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            var room = await _roomService.FindRoomByIdAsync(id);

            if (room is null)
                return NotFound("Room not found.");

            if (!room.IsCompleted)
            {
                return Conflict("Player left the room.");
            }

            if (room.ConfirmFirstPlayer && room.ConfirmSecondPlayer)
                return Ok();

            if (room.IsStartGameTimeOut())
                return Conflict("Time out");

            return NotFound(room.GetStartGameWaitingTime().ToString(@"dd\:mm\:ss"));
        }

        [HttpGet("exit/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ExitFromRoomAsync(string id)
        {
            if (!await FindUser())
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            var work = await _roomService.ExitFromRoomAsync(LoginUser, id);

            return !work ? NotFound("Room is not found.") : Ok();
        }

        [NonAction]
        private async Task<bool> FindUser()
        {
            return await _accountService.IsAccountExistByLoginAsync(LoginUser);
        }

    }
}
