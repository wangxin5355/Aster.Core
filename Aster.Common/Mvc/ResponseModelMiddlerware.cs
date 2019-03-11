using Aster.Common.Extensions;
using Aster.Common.Mvc.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace Aster.Common.Mvc
{
    public static class ResponseModelMiddlerwareExtensions
    {
        public static void UseWrapperResponse(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var bodyStream = context.Response.Body;

                var responseBodyStream = new MemoryStream();
                context.Response.Body = responseBodyStream;

                await next();

                RedirectIfMvcRequestHasError(context, out bool hasRedirect);

                if (!hasRedirect)
                {
                    if (NeedWrapper(context))
                    {
                        responseBodyStream.Seek(0, SeekOrigin.Begin);
                        var responseBody = new StreamReader(responseBodyStream).ReadToEnd();
                        var newBytes = WrapperResponse(context, responseBody).ToBytes();

                        context.Response.ContentLength = newBytes.Length;
                        context.Response.ContentType = "application/json";
                        await bodyStream.WriteAsync(newBytes);
                    }
                    else
                    {
                        responseBodyStream.Seek(0, SeekOrigin.Begin);
                        context.Response.ContentLength = responseBodyStream.Length;
                        await responseBodyStream.CopyToAsync(bodyStream);
                    }
                }
            });
        }

        private static void RedirectIfMvcRequestHasError(HttpContext context, out bool hasRedirect)
        {
            hasRedirect = false;
            if (context.Request.Path.ToString().Contains("/api/"))
                return;
            var code = context.Response.StatusCode.ToString();

            if (code.StartsWith("4") || code.StartsWith("5"))
            {
                hasRedirect = true;
                context.Response.Redirect($"/error/{code}");
                context.Response.StatusCode = StatusCodes.Status301MovedPermanently;
            }
        }

        /// <summary>
        /// 只包装api请求的返回结果
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static bool NeedWrapper(HttpContext context)
        {
            if (context.Response.StatusCode == 204)
            {
                return false;
            }
            if (!context.Request.Path.ToString().Contains("/api/"))
            {
                return false;
            }
            //html页面，不需要处理
            if (context.Response.ContentType != null &&
                (context.Response.ContentType.Contains("text/html") || context.Response.ContentType.Contains("application/vnd")))
            {
                return false;
            }
            return true;
        }

        private static string WrapperResponse(HttpContext context, string origin)
        {
            var response = context.Response;

            if (!response.StatusCode.ToString().StartsWith("2"))
            {
                //如果返回的http statuscode 不等于2开头，说明有错误
                var m = new ApiResponseModel()
                {
                    Ret = -1,
                    ErrCode = response.StatusCode.ToString(),
                    ErrStr = ((HttpStatusCode)response.StatusCode).ToString()
                };

                return JsonConvert.SerializeObject(m);
            }

            if (response.ContentType != null && response.ContentType.Contains("application/json", StringComparison.InvariantCultureIgnoreCase))
            {
                if (origin.Contains("ret", StringComparison.InvariantCultureIgnoreCase) && origin.Contains("errCode", StringComparison.InvariantCultureIgnoreCase))
                {
                    return origin;
                }

                var m = JsonConvert.DeserializeObject(origin);

                return JsonConvert.SerializeObject(new ApiResponseModel<dynamic>(m));
            }
            else
            {
                return JsonConvert.SerializeObject(new ApiResponseModel<object>(origin));
            }
        }
    }
}
