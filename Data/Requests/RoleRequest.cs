using Newtonsoft.Json;

namespace OpenAi.FuncApp.Data.Requests
{
    public class RoleRequest
    {
        [JsonProperty("user", NullValueHandling = NullValueHandling.Ignore)]
        public string User { get; set; }

        [JsonProperty("assistant", NullValueHandling = NullValueHandling.Ignore)]
        public string Assistant { get; set; }
    }
}