using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenAi.FuncApp.Data.Response
{
    public class TextContent
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class Content
    {
        [JsonProperty("index", NullValueHandling = NullValueHandling.Ignore)]
        public int Index { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public TextContent Text { get; set; }
    }

    public class Delta
    {
        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public List<Content> Content { get; set; } = new List<Content>();
    }

    public class DataDelta
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("object", NullValueHandling = NullValueHandling.Ignore)]
        public string Object { get; set; }

        [JsonProperty("delta", NullValueHandling = NullValueHandling.Ignore)]
        public Delta Delta { get; set; } = new Delta();
    }

    public class DataCompleted
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("object", NullValueHandling = NullValueHandling.Ignore)]
        public string Object { get; set; }

        [JsonProperty("created_at", NullValueHandling = NullValueHandling.Ignore)]
        public long CreatedAt { get; set; }

        [JsonProperty("assistant_id", NullValueHandling = NullValueHandling.Ignore)]
        public string AssistantId { get; set; }

        [JsonProperty("thread_id", NullValueHandling = NullValueHandling.Ignore)]
        public string ThreadId { get; set; }

        [JsonProperty("run_id", NullValueHandling = NullValueHandling.Ignore)]
        public string RunId { get; set; }

        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }

        [JsonProperty("incomplete_details", NullValueHandling = NullValueHandling.Ignore)]
        public string IncompleteDetails { get; set; }

        [JsonProperty("incomplete_at", NullValueHandling = NullValueHandling.Ignore)]
        public long? IncompleteAt { get; set; }

        [JsonProperty("completed_at", NullValueHandling = NullValueHandling.Ignore)]
        public long? CompletedAt { get; set; }

        [JsonProperty("role", NullValueHandling = NullValueHandling.Ignore)]
        public string Role { get; set; }

        [JsonProperty("content")]
        public List<Content> Content { get; set; } = new List<Content>();

        [JsonProperty("attachments")]
        public List<object> Attachments { get; set; } = new List<object>();

        [JsonProperty("metadata")]
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    public class ThreadResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class ThreadEventResponse
    {
        public string Event { get; set; }
        public DataDelta DataDelta { get; set; } = new DataDelta();
        public DataCompleted DataCompleted { get; set; } = new DataCompleted();
    }
}