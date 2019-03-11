using Aster.Common.Extensions;
using Aster.Common.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Aster.Common.Mvc
{
    public class RequestModelFilter : IAsyncResourceFilter
    {
        private readonly ILogger<RequestModelFilter> _logger;

        public RequestModelFilter(ILogger<RequestModelFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }

            await TaskParmIfPost(context, next);

            //await WrapperResponse(context);
        }

        private async Task TaskParmIfPost(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Method.Equals("POST", StringComparison.InvariantCultureIgnoreCase))
            {
                await next();
                return;
            }

            bool requestModelValid = true;
            bool bodyRewrited = false;
            
            var initialBody = context.HttpContext.Request.Body;

            try
            {
                //context.HttpContext.Request.EnableRewind();

                using (StreamReader sr = new StreamReader(context.HttpContext.Request.Body))
                {
                    string content = await sr.ReadToEndAsync();
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        var m = JsonConvert.DeserializeObject<JObject>(content);
                        if (!m.TryGetValue("param",out JToken param))
                        {
                            requestModelValid = false;
                        }
                        else
                        {
                            //取ApiRequestModel的Param重新写入request body
                            MemoryStream ms = new MemoryStream(JsonConvert.SerializeObject(param).ToBytes());
                            context.HttpContext.Request.Body = ms;
                            context.HttpContext.Request.ContentLength = ms.Length;
                            bodyRewrited = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "解析Post参数出错");
                requestModelValid = false;
            }
            finally
            {
                if (!bodyRewrited) context.HttpContext.Request.Body = initialBody;
            }
            if (!requestModelValid)
            {
                context.Result = new JsonResult(new ApiResponseModel()
                {
                    Ret = -1,
                    ErrCode = "",
                    ErrStr = "Illegal request"
                });
            }
            else
            {
                await next();
            }
        }
        
    }
}
