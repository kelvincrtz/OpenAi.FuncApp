using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenAi.FuncApp.Data.Requests
{
    public class RunRequest
    {
        [JsonProperty("assistant_id", NullValueHandling = NullValueHandling.Ignore)]
        public string Assistant_Id { get; set; }

        [JsonProperty("additional_instructions", NullValueHandling = NullValueHandling.Ignore)]
        public string Additional_Instructions { get; set; }

        [JsonProperty("tool_choice", NullValueHandling = NullValueHandling.Ignore)]
        public string Tool_Choice { get; set; }
    }
}