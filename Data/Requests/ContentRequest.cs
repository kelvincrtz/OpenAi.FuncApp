using Newtonsoft.Json;

namespace OpenAi.FuncApp.Data.Requests
{
    public class ContentRequest
    {
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
    }
}