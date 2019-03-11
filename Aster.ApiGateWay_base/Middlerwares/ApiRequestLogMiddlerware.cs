
using Aster.Common.Extensions;
using Aster.Common.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aster.ApiGateWay_base.Middlerwares
{
    public class ApiRequestLogMiddlerware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public ApiRequestLogMiddlerware(RequestDelegate next,
            ILogger<ApiRequestLogMiddlerware> logger)
        {
            _logger = logger;
            _next = next;
        }

        /// <summary>
        /// 请求日志记录
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            context.Items["CorrelationId"] = Guid.NewGuid().ToString();

            if (!context.Request.Path.HasValue || !context.Request.Path.Value.Contains("api/", StringComparison.InvariantCultureIgnoreCase))
            {
                await _next(context);
                return;
            }

            var request = await FormatRequest(context.Request);
            var ip = IpUtil.GetClientIp(context);
            var userId = context.User?.GetId();
            var packType = context.Request.GetPackType();
            var converssionType = context.Request.GetConversionCurrency();

            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                Stopwatch watch = Stopwatch.StartNew();

                await _next(context);

                var response = await FormatResponse(context.Response);

                await responseBody.CopyToAsync(originalBodyStream);

                watch.Stop();

                Log(ip, userId, packType.ToString(), converssionType, request, response, watch.ElapsedMilliseconds);
            }
        }

        private void Log(string ip, int? userId, string packType, string converssionCurrency, string request, string response, long costTimes)
        {

            _logger.LogInformation("{IP} {UserId} {PackType} {ConverssionCurrency} {Request} {Response} {Costs}ms",
                ip, userId, packType, converssionCurrency, request, response, costTimes);
        }

            

        private async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableRewind();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            await request.Body.ReadAsync(buffer, 0, buffer.Length);

            var bodyAsText = Encoding.UTF8.GetString(buffer);

            request.Body.Position = 0;

            return $"{request.Path} {request.QueryString} {bodyAsText}";
        }

        private string DelPasswordInfo(string s)
        {
            return Regex.Replace(s, @"""password""\s*\:\s*"".*?""", @"""password"":""*""");
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            if (response.ContentType != null && (
                response.ContentType.Contains("application/json", StringComparison.InvariantCultureIgnoreCase) ||
                response.ContentType.Contains("text/", StringComparison.InvariantCultureIgnoreCase)))
            {
                response.Body.Seek(0, SeekOrigin.Begin);

                string text = await new StreamReader(response.Body).ReadToEndAsync();

                response.Body.Seek(0, SeekOrigin.Begin);

                return $"{response.StatusCode}: {text}";
            }
            else
            {
                response.Body.Seek(0, SeekOrigin.Begin);
                return $"{response.StatusCode}: {response.ContentType}类型";
            }
        }
    }
}
