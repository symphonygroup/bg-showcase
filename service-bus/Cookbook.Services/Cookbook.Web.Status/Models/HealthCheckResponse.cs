using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;

namespace Cookbook.Web.Status.Models
{
    public class HealthCheckResponse
    {
        [JsonProperty("status")]
        public string? Status { get; set; }

        [JsonProperty("results")]
        public Results? Results { get; set; }

    }

    public class Results
    {
        [JsonProperty("masstransit-bus")]
        public MassTransitBus? MassTransitBus { get; set; }
    }

    public class MassTransitBus
    {
        [JsonProperty("status")]
        public string? Status { get; set; }
        [JsonProperty("description")]
        public string? Description { get; set; }
        [JsonProperty("data")]
        public Data? Data { get; set; }
    }

    public class Data
    {
        public JObject Endpoints { get; set; }
    }
}
