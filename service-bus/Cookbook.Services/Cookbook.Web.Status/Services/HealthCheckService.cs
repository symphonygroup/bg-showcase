using Cookbook.Web.Status.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cookbook.Web.Status.Services
{
    public class HealthCheckService : IHealthCheckService
    {
        private readonly HttpClient _httpClient;

        public HealthCheckService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(HealthCheckViewModel? Ready, HealthCheckViewModel? Live)> GetHealthCheck(string url)
        {
            _httpClient.BaseAddress = new Uri(url);

            var readyRequestMessage = new HttpRequestMessage(HttpMethod.Get, "/health/ready");
            var liveRequestMessage = new HttpRequestMessage(HttpMethod.Get, "/health/live");

            var readyResponse = await _httpClient.SendAsync(readyRequestMessage);
            var liveResponse = await _httpClient.SendAsync(liveRequestMessage);

            var readyResponseJson = await readyResponse.Content.ReadAsStringAsync();
            var convertedReadyResponse = JsonConvert.DeserializeObject<HealthCheckResponse>(readyResponseJson);

            var liveResponseJson = await liveResponse.Content.ReadAsStringAsync();
            var convertedLiveResponse = JsonConvert.DeserializeObject<HealthCheckResponse>(liveResponseJson);

            var readyViewModel = new HealthCheckViewModel
            {
                Status = convertedReadyResponse.Status,
                Results = new ResultsViewModel
                {
                    MassTransitBus = new MassTransitBusViewModel
                    {
                        Status = convertedReadyResponse.Results.MassTransitBus.Status,
                        Description = convertedReadyResponse.Results.MassTransitBus.Description,
                        Data = new DataViewModel()
                    }
                }
            };

            var readyEndpoints = new List<bool>();
            foreach (var endpoint in convertedReadyResponse.Results.MassTransitBus.Data.Endpoints)
            {
                var status = endpoint.Value["status"];
                readyEndpoints.Add(status.ToString() == "Healthy");
            }

            readyViewModel.Results.MassTransitBus.Data.Endpoints = readyEndpoints;

            var liveViewModel = new HealthCheckViewModel
            {
                Status = convertedLiveResponse.Status,
                Results = new ResultsViewModel
                {
                    MassTransitBus = new MassTransitBusViewModel
                    {
                        Status = convertedLiveResponse.Results.MassTransitBus.Status,
                        Description = convertedLiveResponse.Results.MassTransitBus.Description,
                        Data = new DataViewModel()
                    }
                }
            };

            var liveEndpoints = new List<bool>();
            foreach (var endpoint in convertedLiveResponse.Results.MassTransitBus.Data.Endpoints)
            {
                var status = endpoint.Value["status"];
                liveEndpoints.Add(status.ToString() == "Healthy");
            }

            liveViewModel.Results.MassTransitBus.Data.Endpoints = liveEndpoints;

            return (readyViewModel, liveViewModel);
        }
    }
}
