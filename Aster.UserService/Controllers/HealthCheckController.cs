using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aster.Security.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aster.UserService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    [ServiceFilter(typeof(WhiteListFilterAttribute))]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet("")]
        [HttpHead("")]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}