using System.Net;
using Microsoft.AspNetCore.Mvc;
using Tic_Tac_Toe.Server.Models;
using Tic_Tac_Toe.Server.Service;

namespace Tic_Tac_Toe.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private ILogger<AccountController> _logger;

        private readonly IAccountService _accService;

        public AccountController(ILogger<AccountController> logger, IAccountService accountService)
        {
            _logger = logger;
            _accService = accountService;
        }

        [HttpPost("/login")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> LoginAsync([FromBody] UserAccount account)
        {
            await _accService.FindAllUsersAccount();

            var storage = _accService.GetStorage();


            return storage.Contains(account) ? Ok(account) : BadRequest(account);
        }




    }
}
