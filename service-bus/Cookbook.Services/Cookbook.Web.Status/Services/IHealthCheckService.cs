using Cookbook.Web.Status.Models;

namespace Cookbook.Web.Status.Services
{
    public interface IHealthCheckService
    {
        Task<(HealthCheckViewModel? Ready, HealthCheckViewModel? Live)> GetHealthCheck(string url);
    }
}
