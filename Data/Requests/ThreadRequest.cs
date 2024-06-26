using Newtonsoft.Json;

namespace OpenAi.FuncApp.Data.Requests
{
    public class ThreadRequest
    {
        [JsonProperty("metadata", NullValueHandling = NullValueHandling.Ignore)]
        public Metadata Metadata { get; set; }
    }

    public class Metadata
    {
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("user", NullValueHandling = NullValueHandling.Ignore)]
        public string User { get; set; }
    }
}