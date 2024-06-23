using Newtonsoft.Json;

namespace OpenAi.FuncApp.Data.Response
{
    public class MessageResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("created_at")]
        public int Created_At { get; set; }

        [JsonProperty("assistant_id", NullValueHandling = NullValueHandling.Ignore)]
        public string Assistant_Id { get; set; }

        [JsonProperty("thread_Id")]
        public string Thread_Id { get; set; }

        [JsonProperty("run_id")]
        public string Run_Id { get; set; }
    }
}