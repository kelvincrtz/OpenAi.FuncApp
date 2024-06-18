using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenAi.FuncApp.Data.Requests
{
    public class AssistantRequest
    {
        [JsonProperty("instructions", NullValueHandling = NullValueHandling.Ignore)]
        public string Instructions { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
        public string Model { get; set; }

        [JsonProperty("tools", NullValueHandling = NullValueHandling.Ignore)]
        public List<Tool> Tools { get; set; }
    }

    public class Tool
    {
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
    }
}