using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenAi.FuncApp.Data.Response
{
    public class FileCitation
    {
        [JsonProperty("file_id")]
        public string FileId { get; set; }
    }

    public class Annotation
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("start_index")]
        public int StartIndex { get; set; }

        [JsonProperty("end_index")]
        public int EndIndex { get; set; }

        [JsonProperty("file_citation")]
        public FileCitation FileCitation { get; set; }
    }

    public class TextContentMessage
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("annotations")]
        public List<Annotation> Annotations { get; set; }
    }

    public class ContentMessage
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("text")]
        public TextContentMessage Text { get; set; }
    }

    public class Message
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("created_at")]
        public long CreatedAt { get; set; }

        [JsonProperty("assistant_id")]
        public string AssistantId { get; set; }

        [JsonProperty("thread_id")]
        public string ThreadId { get; set; }

        [JsonProperty("run_id")]
        public string RunId { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("content")]
        public List<ContentMessage> Content { get; set; }

        [JsonProperty("attachments")]
        public List<object> Attachments { get; set; }

        [JsonProperty("metadata")]
        public Dictionary<string, object> Metadata { get; set; }
    }

    public class MessageListResponse
    {
        [JsonProperty("data")]
        public List<Message> Message { get; set; }
    }
}