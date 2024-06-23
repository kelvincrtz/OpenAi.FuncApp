using Newtonsoft.Json;

namespace OpenAi.FuncApp.Data.Requests
{
    public class MessageRequest
    {
        [JsonProperty("role", NullValueHandling = NullValueHandling.Ignore)]
        public string Role { get; set; }

        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public string Content { get; set; }

        [JsonProperty("assistant_id", NullValueHandling = NullValueHandling.Ignore)]
        public string Assistant_Id { get; set; }
    }
}