using Newtonsoft.Json;

namespace CargoGateway.Core.Models;

public class AvailabilitySearchRequest
{
        [JsonProperty("from")]
        public string From { get; set; } = string.Empty;
        [JsonProperty("to")]
        public string To { get; set; } = string.Empty;
        [JsonProperty("date")]
        public DateOnly Date { get; set; }
}