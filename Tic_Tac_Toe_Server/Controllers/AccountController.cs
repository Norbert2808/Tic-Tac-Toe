using System.Net;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Server.Models;
using TicTacToe.Server.Services;
using TicTacToe.Server.Tools;

namespace TicTacToe.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;

        private readonly IAccountService _accService;

        private readonly IBlocker _blocker;

        public AccountController(ILogger<AccountController> logger,
            IAccountService accountService,
            IBlocker blocker)
        {
            _logger = logger;
            _accService = accountService;
            _blocker = blocker;
        }

        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> LoginAsync([FromBody] UserAccount account)
        {
            await _accService.UpdateAllUsersAccountAsync();

            var loginIsExist = _accService.FindAccountByLogin(account.Login);

            if (!loginIsExist)
                return NotFound("Input login does not exist");

            if (_blocker.IsBlocked(account.Login))
            {
                _logger.LogWarning($"User in black list {account.Login}");
                return Unauthorized(account.Login);   
            }

            var passwordIsExist = _accService.FindAccountByPassword(account.Password);

            if (!passwordIsExist)
            {
                _blocker.ErrorTryLogin(account.Login);
                return NotFound("Password is wrong!");
            }

            _blocker.UnBlock(account.Login);
            return Ok(account.Login);
        }

        [HttpPost("registration")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> RegistrationAsync([FromBody] UserAccount account)
        {
            await _accService.UpdateAllUsersAccountAsync();

            if (_accService.FindAccountByLogin(account.Login))
            {
                _logger.LogInformation($"Input login already exists {account.Login}");
                return Conflict("User with such login already registered");
            }

            await _accService.AddAccountToStorageAsync(account);
            return Ok(account.Login);
        }


    }
}
