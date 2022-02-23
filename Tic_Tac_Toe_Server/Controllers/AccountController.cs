using System.Net;
using Microsoft.AspNetCore.Mvc;
using Tic_Tac_Toe.Server.Models;
using Tic_Tac_Toe.Server.Services;
using Tic_Tac_Toe.Server.Tools;

namespace Tic_Tac_Toe.Server.Controllers
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
            var loginIsExist = await _accService.FindAccountByLogin(account.Login);

            if (!loginIsExist)
                return NotFound("Input login does not exist");

            if (_blocker.IsBlocked(account.Login))
                return Unauthorized(account.Login);

            var passwordIsExist = await _accService.FindAccountByPassword(account.Password);

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
        public async Task<IActionResult> RegistrationAsync([FromBody] UserAccount account)
        {
            if (await _accService.FindAccountByLogin(account.Login))
            {
                _logger.LogInformation("Input login already exists");
                return BadRequest("Input login already exists");
            }

            await _accService.AddAccountToStorage(account);
            return Ok(account.Login);
        }


    }
}
