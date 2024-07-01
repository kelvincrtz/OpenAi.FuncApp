using Newtonsoft.Json;

namespace OpenAi.FuncApp.Data.Response
{
    public class TranscriptionResponse
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}