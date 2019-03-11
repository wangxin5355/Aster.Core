using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Aster.Common.Filter
{
    public class MyExceptionFilter : ExceptionFilterAttribute
    {
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<MyExceptionFilter> _logger;
        private readonly IStringLocalizer<MyExceptionFilter> _localizer;

        public MyExceptionFilter(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment,
            IStringLocalizer<MyExceptionFilter> localizer,
            ILogger<MyExceptionFilter> logger)
        {
            _logger = logger;
            _localizer = localizer;
            _hostingEnvironment = hostingEnvironment;
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, $"出现错误.{System.Diagnostics.Activity.Current?.Id ?? context.HttpContext.TraceIdentifier }");


            context.ExceptionHandled = true;

            context.Result = ExceptionHandler.ExceptionToJson(context.Exception, _localizer);
        }
    }
}
