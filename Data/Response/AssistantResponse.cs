using System.Collections.Generic;
using Newtonsoft.Json;
using OpenAi.FuncApp.Helpers;

namespace OpenAi.FuncApp.Data.Response
{
    public class AssistantListResponse
    {
        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("data")]
        public List<Assistant> Data { get; set; }
    }

    public class Assistant
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("created_at")]
        public long CreatedAt { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("instructions")]
        public string Instructions { get; set; }

        [JsonProperty("tools")]
        public List<Tool> Tools { get; set; }

        [JsonProperty("top_p")]
        public double TopP { get; set; }

        [JsonProperty("temperature")]
        public double Temperature { get; set; }

        [JsonProperty("tool_resources")]
        public Dictionary<string, object> ToolResources { get; set; }

        [JsonProperty("metadata")]
        public Dictionary<string, object> Metadata { get; set; }

        [JsonProperty("response_format", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ResponseFormatConverter))]
        public ResponseFormat ResponseFormat { get; set; }
    }

    public class Tool
    {
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class ResponseFormat
    {
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}