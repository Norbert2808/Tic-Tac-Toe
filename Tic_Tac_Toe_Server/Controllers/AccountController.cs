using System.Net;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Server.DTO;
using TicTacToe.Server.Exceptions;
using TicTacToe.Server.Services;

namespace TicTacToe.Server.Controllers
{
    [Route("api/account")]
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
        public async Task<IActionResult> LoginAsync([FromBody] UserAccountDto account)
        {
            try
            {
                LogInformationAboutClass(nameof(LoginAsync),"Invoke log-in");
                await _accService.InvokeLoginAsync(account);
            }
            catch (AccountException exception)
            {
                _logger.LogWarning("Message: {Message}", exception.Message);
                return NotFound(exception.Message);
            }
            catch (TimeoutException exception)
            {
                _logger.LogWarning("Message: {Message}", exception.Message);
                return Conflict(exception.Message);
            }

            return Ok(account.Login);
        }

        [HttpPost("registration")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> RegistrationAsync([FromBody] UserAccountDto account)
        {
            try
            {
                LogInformationAboutClass(nameof(RegistrationAsync), "Invoke registration");
                await _accService.InvokeRegistrationAsync(account);
            }
            catch (AccountException exception)
            {
                _logger.LogWarning("Message: {Message}:", exception.Message);
                return Conflict(exception.Message);
            }

            return Ok(account.Login);
        }

        [HttpPost("logout")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> LogoutAsync([FromBody] string login)
        {
            LogInformationAboutClass(nameof(LogoutAsync), "Invoke logout form app");
            _accService.RemoveActiveAccountByLogin(login);

            return await Task.FromResult(Ok("User successfully left."));
        }

        [NonAction]
        private void LogInformationAboutClass(string methodName, string message)
        {
            _logger.LogInformation("{ClassName}::{MethodName}::{Message}",
                nameof(AccountController), methodName, message);
        }
    }
}
