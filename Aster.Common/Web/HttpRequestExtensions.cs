using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Aster.Common.Web
{
    public static class HttpRequestExtensions
    {

        /// <summary>
        /// Retrieve the raw body as a string from the Request.Body stream
        /// </summary>
        /// <param name="request">Request instance to apply to</param>
        /// <param name="encoding">Optional - Encoding, defaults to UTF8</param>
        /// <returns></returns>
        public static async Task<string> GetRawBodyStringAsync(this HttpRequest request, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            request.EnableRewind();

            string content = null;

            using (MemoryStream ms = new MemoryStream())
            {
                await request.Body.CopyToAsync(ms);

                using (StreamReader sr = new StreamReader(ms, encoding))
                {
                    content = await sr.ReadToEndAsync();
                }
            }

            request.Body.Seek(0, SeekOrigin.Begin);

            return content;
        }
    }
}
