using Newtonsoft.Json;

namespace OpenAi.FuncApp.Data.Requests
{
    public class TranscriptionRequest
    {

        [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
        public string Model { get; set; }

        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }

        [JsonProperty("prompt", NullValueHandling = NullValueHandling.Ignore)]
        public string Prompt { get; set; }

        [JsonProperty("response_format", NullValueHandling = NullValueHandling.Ignore)]
        public string ResponseFormat { get; set; }

        [JsonProperty("temperature", NullValueHandling = NullValueHandling.Ignore)]
        public string Temperature { get; set; }
    }
}