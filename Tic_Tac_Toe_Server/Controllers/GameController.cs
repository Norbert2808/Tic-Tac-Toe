using System.Net;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Server.DTO;
using TicTacToe.Server.Exceptions;
using TicTacToe.Server.Services;

namespace TicTacToe.Server.Controllers
{
    [Route("api/game")]
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

        [HttpPost("create-room")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> StartRoomAsync([FromBody] RoomSettingsDto? settings)
        {
            LogInformationAboutClass(nameof(StartRoomAsync), $"Processing request: {Request.Path}");
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

            LogInformationAboutClass(nameof(StartRoomAsync),
                $"Invoke method::{nameof(_roomService.StartRoomAsync)}");

            try
            {
                response = await _roomService.StartRoomAsync(settings.RoomId, LoginUser, settings);
            }
            catch (RoomException exception)
            {
                _logger.LogWarning("Message: {Message}:", exception.Message);
                return BadRequest(exception.Message);
            }

            return Ok(response);
        }

        [HttpGet("check-room/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> CheckRoomAsync([FromRoute] string id)
        {
            LogInformationAboutClass(nameof(CheckRoomAsync), $"Processing request: {Request.Path}");
            if (string.IsNullOrEmpty(LoginUser))
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            LogInformationAboutClass(nameof(CheckRoomAsync),
                $"Invoke method::{nameof(_roomService.CheckRoomAsync)}");

            try
            {
                var (isCompleted, message) = await _roomService.CheckRoomAsync(id);
                return isCompleted ? Ok() : NotFound(message);
            }
            catch (RoomException exception)
            {
                _logger.LogWarning("Message: {Message}", exception.Message);
                return BadRequest(exception.Message);
            }
            catch (TimeoutException exception)
            {
                _logger.LogWarning("Message: {Message}", exception.Message);
                return Conflict(exception.Message);
            }
        }

        [HttpGet("check-move/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> CheckMoveAsync([FromRoute] string id)
        {
            LogInformationAboutClass(nameof(CheckMoveAsync), $"Processing request: {Request.Path}");
            if (string.IsNullOrEmpty(LoginUser))
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            LogInformationAboutClass(nameof(CheckMoveAsync),
                $"Invoke method::{nameof(_roomService.CheckMoveAsync)}");

            try
            {
                var roundState = await _roomService.CheckMoveAsync(id, LoginUser);
                return roundState is null ? NotFound() : Ok(roundState);
            }
            catch (RoomException exception)
            {
                _logger.LogWarning("Message: {Message}", exception.Message);
                return NotFound(exception.Message);
            }
            catch (TimeoutException exception)
            {
                _logger.LogWarning("Message: {Message}", exception.Message);
                return Conflict(exception.Message);
            }
            catch (GameException exception)
            {
                return Conflict(exception.Message);
            }
        }

        [HttpPost("move/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> MoveAsync([FromRoute] string id, [FromBody] MoveDto move)
        {
            LogInformationAboutClass(nameof(MoveAsync), $"Processing request: {Request.Path}");
            if (string.IsNullOrEmpty(LoginUser))
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            LogInformationAboutClass(nameof(MoveAsync),
                $"Invoke method::{nameof(_roomService.DoMoveAsync)}");

            try
            {
                await _roomService.DoMoveAsync(id, LoginUser, move);
                return Ok();
            }
            catch (RoomException exception)
            {
                _logger.LogWarning("Message: {Message}", exception.Message);
                return NotFound(exception.Message);
            }
            catch (ArgumentException exception)
            {
                _logger.LogWarning("Message: {Message}", exception.Message);
                return BadRequest(exception.Message);
            }
            catch (TimeoutException exception)
            {
                _logger.LogWarning("Message: {Message}", exception.Message);
                return Conflict(exception.Message);
            }
        }

        [HttpPost("send-confirmation/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> AppendConfirmationAsync([FromRoute] string id, [FromBody] bool confirmation)
        {
            LogInformationAboutClass(nameof(AppendConfirmationAsync), $"Processing request: {Request.Path}");
            if (string.IsNullOrEmpty(LoginUser))
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            LogInformationAboutClass(nameof(AppendConfirmationAsync),
                $"Invoke method {nameof(_roomService.AppendConfirmationAsync)}");

            try
            {
                await _roomService.AppendConfirmationAsync(confirmation, id);
            }
            catch (RoomException exception)
            {
                _logger.LogWarning("Message: {Message}", exception.Message);
                return Conflict(exception.Message);
            }
            catch (TimeoutException exception)
            {
                _logger.LogWarning("Message: {Message}", exception.Message);
                return Conflict(exception.Message);
            }

            return Ok();
        }

        [HttpGet("check-confirmation/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> CheckConfirmationAsync([FromRoute] string id)
        {
            LogInformationAboutClass(nameof(CheckConfirmationAsync), $"Processing request: {Request.Path}");
            if (string.IsNullOrEmpty(LoginUser))
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            LogInformationAboutClass(nameof(CheckConfirmationAsync),
                $"Invoke method {nameof(_roomService.CheckConfirmationAsync)}");

            try
            {
                var (isConfirm, message) = await _roomService.CheckConfirmationAsync(id, LoginUser);
                return isConfirm ? Ok() : NotFound(message);
            }
            catch (RoomException exception)
            {
                _logger.LogWarning("Message: {Message}", exception.Message);
                return Conflict(exception.Message);
            }
            catch (TimeoutException exception)
            {
                _logger.LogWarning("Message: {Message}", exception.Message);
                return Conflict(exception.Message);
            }
        }

        [HttpGet("get-results/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetResultsAsync([FromRoute] string id)
        {
            LogInformationAboutClass(nameof(GetResultsAsync), $"Processing request: {Request.Path}");
            if (string.IsNullOrEmpty(LoginUser))
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            LogInformationAboutClass(nameof(GetResultsAsync),
                $"Invoke method {nameof(_roomService.GetResultAsync)}");

            try
            {
                var results = await _roomService.GetResultAsync(id);
                return Ok(results);
            }
            catch (RoomException exception)
            {
                _logger.LogWarning("Message: {Message}", exception.Message);
                return NotFound(exception.Message);
            }
        }

        [HttpGet("check-round-state/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> CheckStateRoundAsync([FromRoute] string id)
        {
            LogInformationAboutClass(nameof(CheckStateRoundAsync), $"Processing request: {Request.Path}");
            if (string.IsNullOrEmpty(LoginUser))
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            LogInformationAboutClass(nameof(CheckStateRoundAsync),
                $"Invoke method {nameof(_roomService.CheckStateRoundAsync)}");

            try
            {
                var roundState = await _roomService.CheckStateRoundAsync(id, LoginUser);
                return Ok(roundState);
            }
            catch (RoomException exception)
            {
                _logger.LogWarning("Message: {Message}", exception.Message);
                return NotFound(exception.Message);
            }
        }

        [HttpDelete("exit/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ExitFromRoomAsync([FromRoute] string id)
        {
            LogInformationAboutClass(nameof(ExitFromRoomAsync), $"Processing request: {Request.Path}");
            if (string.IsNullOrEmpty(LoginUser))
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            LogInformationAboutClass(nameof(ExitFromRoomAsync),
                $"Invoke method {nameof(_roomService.ExitFromRoomAsync)}");

            try
            {
                await _roomService.ExitFromRoomAsync(id);
                return Ok();
            }
            catch (RoomException exception)
            {
                _logger.LogWarning("Message: {Message}", exception.Message);
                return NotFound(exception.Message);
            }
        }

        [HttpGet("surrender/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SurrenderAsync([FromRoute] string id)
        {
            LogInformationAboutClass(nameof(SurrenderAsync), $"Processing request: {Request.Path}");
            if (string.IsNullOrEmpty(LoginUser))
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            LogInformationAboutClass(nameof(SurrenderAsync),
                $"Invoke method {nameof(_roomService.SurrenderAsync)}");

            try
            {
                await _roomService.SurrenderAsync(id, LoginUser);
            }
            catch (RoomException exception)
            {
                _logger.LogWarning("Message: {Message}", exception.Message);
                return BadRequest(exception.Message);
            }

            return Ok();
        }

        [NonAction]
        private void LogInformationAboutClass(string methodName, string message)
        {
            _logger.LogInformation("{ClassName}::{MethodName}::{Message}",
                nameof(GameController), methodName, message);
        }

    }
}
