using Aster.Common.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Aster.Common.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// 获取当前请求的客户端设备类型
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static PackTypeEnum GetPackType(this HttpRequest request)
        {
            var heads = request.Headers;
            if (heads.TryGetValue("packType", out StringValues t))
            {
                return ((string)t).ToPackType() ?? PackTypeEnum.PcWeb;
            }
            return PackTypeEnum.PcWeb;
        }

        /// <summary>
        /// 获取计价货币(CNY, USD)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetConversionCurrency(this HttpRequest request)
        {
            var conversionCurrency = "USD";

            if (request.Headers.TryGetValue("ConversionCurrency", out StringValues t) && !string.IsNullOrWhiteSpace(t))
            {
                conversionCurrency = t;
            }

            if (conversionCurrency != "USD" && conversionCurrency != "CNY")
                conversionCurrency = "USD";

            return conversionCurrency;
        }

    }
}
