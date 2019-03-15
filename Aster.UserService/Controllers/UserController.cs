using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aster.Common.Extensions;
using Aster.Common.Mvc.Models;
using Aster.Services;
using Aster.Services.Models;
using Aster.UserService.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Aster.UserService.Controllers
{
    [Route("api/user/[action]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {

        private readonly ILogger _logger;
        private readonly IUserService _userService;
        public UserController(ILogger<UserController> logger, IUserService userService)
        {

            _logger = logger;
            _userService = userService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel m)
        {
            var rm = await _userService.Login(m.UserName, m.Password, Request.GetPackType().ToString());
            return Ok(new ApiResponseModel<LoginRpsModel>(rm));
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
