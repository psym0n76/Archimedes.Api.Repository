using Newtonsoft.Json;

namespace Archimedes.Api.Repository
{
    public class TradeDto
    {
        [JsonProperty(PropertyName = "market")]
        public string Market { get; set; }

        [JsonProperty(PropertyName = "direction")]
        public string Direction { get; set; }
    }
}