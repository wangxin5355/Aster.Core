using Aster.Common.Exceptions;
using Aster.Common.Utils;
using Aster.Security.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aster.Security.Models
{
    public class WhiteListFilterAttribute : ActionFilterAttribute
    {
        private readonly TokenOptions _options;

        public WhiteListFilterAttribute(IOptionsSnapshot<TokenOptions> options)
        {
            _options = options.Value;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string ip = IpUtil.GetClientIp(context.HttpContext);

            if (string.IsNullOrWhiteSpace(ip))
                throw new MyException($"无法取到来源IP，不允许访问");

            if (_options.IpWhiteList == null || _options.IpWhiteList.Count == 0)
                throw new MyException("无法读取IP白名单配置");

            if (!_options.IpWhiteList.Contains(ip))
                throw new MyException($"IP[{ip}]无权范围");

            await next.Invoke();
        }
    }
}
