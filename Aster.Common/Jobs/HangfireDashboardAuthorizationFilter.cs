using Hangfire.Dashboard;
using System.Linq;

namespace Aster.Common.Jobs
{
    /// <summary>
    /// hangfire dashboard 验证权限filter
    /// </summary>
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            if (!httpContext.User.Identity.IsAuthenticated)
                return false;

            bool isAdmin = httpContext.User.Claims.FirstOrDefault(x => x.Type == "isAdmin")?.Value == "True";

            return isAdmin;
        }
    }
}
