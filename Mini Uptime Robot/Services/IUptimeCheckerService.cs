using Mini_Uptime_Robot.Models;

namespace Mini_Uptime_Robot.Services
{
    public interface IUptimeCheckerService
    {
        Task<WebsiteResult> CheckAsync(string url, CancellationToken cancellationToken);
    }
}
