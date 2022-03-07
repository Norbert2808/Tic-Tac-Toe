using System.Net;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Server.Exceptions;
using TicTacToe.Server.Models;
using TicTacToe.Server.Services;

namespace TicTacToe.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;

        private readonly IAccountService _accService;

        public AccountController(ILogger<AccountController> logger,
            IAccountService accountService)
        {
            _logger = logger;
            _accService = accountService;
        }

        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> LoginAsync([FromBody] UserAccount account)
        {
            try
            {
                _logger.LogInformation("Invoke log-in state");
                await _accService.InvokeLoginAsync(account);
            }
            catch (AccountException exception)
            {
                _logger.LogInformation(exception.Message);
                return NotFound(exception.Message);
            }
            catch (TimeoutException exception)
            {
                _logger.LogInformation(exception.Message);
                return Conflict(exception.Message);
            }

            return Ok(account.Login);
        }

        [HttpPost("registration")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> RegistrationAsync([FromBody] UserAccount account)
        {
            try
            {
                _logger.LogInformation("Invoke registration state");
                await _accService.InvokeRegistrationAsync(account);
            }
            catch (AccountException exception)
            {
                _logger.LogInformation(exception.Message);
                return Conflict(exception.Message);
            }

            return Ok(account.Login);
        }

        [HttpPost("logout")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> LogoutAsync([FromBody] string login)
        {
            _accService.RemoveActiveAccountByLogin(login);

            return await Task.FromResult(Ok("User successfully left."));
        }
    }
}
