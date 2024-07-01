using Newtonsoft.Json;

namespace OpenAi.FuncApp.Data.Requests
{
    public class SpeechRequest
    {
        [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
        public string Model { get; set; }

        [JsonProperty("input", NullValueHandling = NullValueHandling.Ignore)]
        public string Input { get; set; }

        [JsonProperty("voice", NullValueHandling = NullValueHandling.Ignore)]
        public string Voice { get; set; }

        [JsonProperty("response_format", NullValueHandling = NullValueHandling.Ignore)]
        public string ResponseFormat { get; set; }
    }
}