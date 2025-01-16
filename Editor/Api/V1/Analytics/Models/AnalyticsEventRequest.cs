using System.Collections.Generic;
using Newtonsoft.Json;

namespace PlayerZero.Editor.Api.V1.Analytics.Models
{
    public class AnalyticsEventRequest
    {
        [JsonProperty("data")]
        public AnalyticsEventRequestBody Payload { get; set; } = new AnalyticsEventRequestBody();
    }

    public class AnalyticsEventRequestBody
    {
        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }
}