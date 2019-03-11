using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aster.Security.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aster.TradeService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet("")]
        [HttpHead("")]
        [AllowAnonymous]
        [ServiceFilter(typeof(WhiteListFilterAttribute))]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}