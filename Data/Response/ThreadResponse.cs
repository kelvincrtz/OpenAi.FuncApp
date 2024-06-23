using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenAi.FuncApp.Data.Response
{
    // TODO REMOVE
    public class ThreadResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("created_at")]
        public int Created_At { get; set; }

        [JsonProperty("metadata", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Metadata { get; set; }

        [JsonProperty("tool_resources", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Tool_Resources { get; set; }
    }
}