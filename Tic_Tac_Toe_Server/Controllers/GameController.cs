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
        public string LoginUser { get; set; }
        
        [HttpPost("create_room")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> StartRoomAsync([FromBody] RoomSettings? settings)
        {
            if (LoginUser is null or "")
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
            if (LoginUser is null or "")
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            var room = await _roomService.FindRoomByIdAsync(id);

            if (room is null)
                return BadRequest("Room not found.");

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
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> CheckMoveAsync([FromRoute] string id)
        {
            if (LoginUser is null or "")
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }
            var room = await _roomService.FindRoomByIdAsync(id);
 
            if (room is null)
                 return NotFound("Room not found.");
 
            var round = room.Rounds.Peek();
            var isFirstPlayer = room.LoginFirstPlayer.Equals(LoginUser, StringComparison.Ordinal);
            var rightMove = round.CheckOpponentsMove(isFirstPlayer);

            if (rightMove)
            {
                return round.CheckEndOfGame()
                    ? Accepted(round.LastMove)
                    : Ok(round.LastMove);
            }

            return BadRequest();
        }

        [HttpPost("move/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> MoveAsync([FromRoute] string id, [FromBody] Move move)
        {
            if (LoginUser is null or "")
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            var room = await _roomService.FindRoomByIdAsync(id);

            if (room is null)
                return NotFound("Room not found.");

            var round = room.Rounds.Peek();

            var isFirstPlayer = room.LoginFirstPlayer.Equals(LoginUser, StringComparison.Ordinal);
            try
            {
                var isValid = round.DoMove(move, isFirstPlayer);
                if (isValid)
                {
                    if (round.CheckEndOfGame())
                    {
                        room.ConfirmFirstPlayer = false;
                        room.ConfirmSecondPlayer = false;
                        return Accepted();
                    }

                    return Ok();
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            return NotFound();
        }

        [HttpPost("send_confirmation/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> AppendConfirmationAsync([FromRoute]string id, [FromBody] bool confirmation)
        {
            if (LoginUser is null or "")
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
        public async Task<IActionResult> CheckConfirmationAsync([FromRoute] string id)
        {
            if (LoginUser is null or "")
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
            {
                room.Rounds.Push(new Round());
                return Ok();   
            }

            if (room.IsStartGameTimeOut())
                return Conflict("Time out");

            return NotFound(room.GetStartGameWaitingTime().ToString(@"dd\:mm\:ss"));
        }
        
        [HttpGet("check_position/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> CheckPlayerPositionAsync([FromRoute] string id)
        {
            if (LoginUser is null or "")
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            var room = await _roomService.FindRoomByIdAsync(id);

            if (room is null)
                return NotFound("Room not found.");

            var isFirst = room.LoginFirstPlayer.Equals(LoginUser, StringComparison.Ordinal);
            
            return Ok(isFirst);
        }

        [HttpGet("exit/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ExitFromRoomAsync([FromRoute] string id)
        {
            if (LoginUser is null or "")
            {
                _logger.LogWarning("Unauthorized users");
                return Unauthorized("Unauthorized users");
            }

            var work = await _roomService.ExitFromRoomAsync(LoginUser, id);

            return !work ? NotFound("Room is not found.") : Ok();
        }

    }
}
