using System.Net;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Server.Exceptions;
using TicTacToe.Server.Models;
using TicTacToe.Server.Services;

namespace TicTacToe.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IRoomService _roomService;

        private readonly ILogger<GameController> _logger;

        public GameController(IRoomService roomService,
            ILogger<GameController> logger)
        {
            _roomService = roomService;
            _logger = logger;
        }

        [FromHeader(Name = "Login")]

        public string? LoginUser { get; set; }

        [HttpPost("create_room")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> StartRoomAsync([FromBody] RoomSettingsDto? settings)
        {
            if (string.IsNullOrEmpty(LoginUser))
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            if (settings is null)
            {
                _logger.LogWarning("Settings is null");
                return BadRequest("Settings is null");
            }
            string response;

            try
            {
                response = await _roomService.StartRoomAsync(settings.RoomId, LoginUser, settings);
            }
            catch (RoomException exception)
            {
                return BadRequest(exception.Message);
            }

            return Ok(response);
        }

        [HttpGet("check_room/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> CheckRoomAsync([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(LoginUser))
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            try
            {
                var (isCompleted, message) = await _roomService.CheckRoomAsync(id);
                return isCompleted ? Ok(message) : NotFound(message);
            }
            catch (RoomException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (TimeoutException exception)
            {
                return Conflict(exception.Message);
            }
        }

        [HttpGet("check_move/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> CheckMoveAsync([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(LoginUser))
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            try
            {
                var (isFinished, lastMove) = await _roomService.CheckMoveAsync(id, LoginUser);

                return isFinished ? Accepted(lastMove) : Ok(lastMove);
            }
            catch (RoomException exception)
            {
                return NotFound(exception.Message);
            }
            catch (TimeOutException exception)
            {
                return Conflict(exception.Message);
            }
        }

        [HttpPost("move/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> MoveAsync([FromRoute] string id, [FromBody] MoveDto move)
        {
            if (string.IsNullOrEmpty(LoginUser))
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            try
            {
                var isFinished = await _roomService.DoMoveAsync(id, LoginUser, move);
                return isFinished ? Accepted() : Ok();
            }
            catch (RoomException exception)
            {
                return NotFound(exception.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (TimeOutException exception)
            {
                return Conflict(exception.Message);
            }
        }

        [HttpPost("send_confirmation/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> AppendConfirmationAsync([FromRoute] string id, [FromBody] bool confirmation)
        {
            if (string.IsNullOrEmpty(LoginUser))
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            try
            {
                await _roomService.AppendConfirmationAsync(confirmation, id);
            }
            catch (RoomException exception)
            {
                return NotFound(exception.Message);
            }
            catch (TimeoutException exception)
            {
                return Conflict(exception.Message);
            }

            return Ok();
        }

        [HttpGet("check_confirmation/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> CheckConfirmationAsync([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(LoginUser))
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            try
            {
                var (isConfirm, message) = await _roomService.CheckConfirmationAsync(id);
                return isConfirm ? Ok() : NotFound(message);
            }
            catch (RoomException exception)
            {
                return NotFound(exception.Message);
            }
            catch (AccountException exception)
            {
                return Conflict(exception.Message);
            }
            catch (TimeoutException exception)
            {
                return Conflict(exception.Message);
            }
        }

        [HttpGet("check_position/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> CheckPlayerPositionAsync([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(LoginUser))
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            try
            {
                var isFirst = await _roomService.CheckPlayerPositionAsync(id, LoginUser);
                return Ok(isFirst);
            }
            catch (RoomException exception)
            {
                return NotFound(exception.Message);
            }
            catch (AccountException)
            {
                return Conflict();
            }
        }

        [HttpGet("exit/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ExitFromRoomAsync([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(LoginUser))
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            var isExit = await _roomService.ExitFromRoomAsync(LoginUser, id);

            return !isExit ? NotFound("Room is not found.") : Ok();
        }

        [HttpGet("surrender/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SurrenderAsync([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(LoginUser))
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            try
            {
                await _roomService.SurrenderAsync(id, LoginUser);
            }
            catch (RoomException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

    }
}
