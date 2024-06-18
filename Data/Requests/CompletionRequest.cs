using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenAi.FuncApp.Data.Requests
{
    public class CompletionRequest
    {
        [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
        public string Model { get; set; }

        [JsonProperty("prompt", NullValueHandling = NullValueHandling.Ignore)]
        public string Prompt { get; set; }

        [JsonProperty("maxTokens", NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxTokens { get; set; }

        [JsonProperty("temperature", NullValueHandling = NullValueHandling.Ignore)]
        public double? Temperature { get; set; }

        [JsonProperty("messages", NullValueHandling = NullValueHandling.Ignore)]
        public List<Message> Messages { get; set; }
    }

    public class Message
    {
        [JsonProperty("role", NullValueHandling = NullValueHandling.Ignore)]
        public string Role { get; set; }

        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public string Content { get; set; }
    }
}