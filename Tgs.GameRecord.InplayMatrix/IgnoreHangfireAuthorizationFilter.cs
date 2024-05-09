using Hangfire.Dashboard;

namespace Tgs.GameRecord.InplayMatrix;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context) => true;
}
