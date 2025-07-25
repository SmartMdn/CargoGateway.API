using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CargoGateway.Application.DTO;

public class AvailabilitySearchRequest
{
        [Required]
        [StringLength(10, MinimumLength = 3)]
        [JsonPropertyName("from")]
        public string From { get; set; } = string.Empty;

        [Required]
        [StringLength(10, MinimumLength = 3)]
        [JsonPropertyName("to")]
        public string To { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("date")]
        public DateOnly Date { get; set; }
}