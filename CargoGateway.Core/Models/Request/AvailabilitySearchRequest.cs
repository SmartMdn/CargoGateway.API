using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace CargoGateway.Core.Models.Request;

public class AvailabilitySearchRequest
{
        [Required]
        [StringLength(10, MinimumLength = 3)]
        [JsonProperty("from")]
        public string From { get; set; } = string.Empty;

        [Required]
        [StringLength(10, MinimumLength = 3)]
        [JsonProperty("to")]
        public string To { get; set; } = string.Empty;

        [Required]
        [JsonProperty("date")]
        public string DateString { get; set; } = string.Empty; // Изменено на string

        [JsonIgnore]
        public DateOnly Date
        {
                get => DateOnly.Parse(DateString);
                set => DateString = value.ToString("yyyy-MM-dd");
        }
}