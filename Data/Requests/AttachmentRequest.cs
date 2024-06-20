using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenAi.FuncApp.Data.Requests
{
    public class AttachmentRequest
    {
        [JsonProperty("file_id", NullValueHandling = NullValueHandling.Ignore)]
        public string File_id { get; set; }

        [JsonProperty("tools", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Tools { get; set; } // TODO
    }
}