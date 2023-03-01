namespace Cookbook.Web.Status.Models
{
    public class HealthCheckViewModel
    {
        public string Status { get; set; }
        public ResultsViewModel Results { get; set; }
    }

    public class ResultsViewModel
    {
        public MassTransitBusViewModel MassTransitBus { get; set; }
    }

    public class MassTransitBusViewModel
    {
        public string Status { get; set; }
        public string Description { get; set; }
        public DataViewModel Data { get; set; }
    }

    public class DataViewModel
    {
        public List<bool> Endpoints { get; set; }
    }
}
