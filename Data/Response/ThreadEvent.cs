using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenAi.FuncApp.Data.Response
{
    public class TextContent
    {
        public string Value { get; set; }
    }

    public class Content
    {
        public int Index { get; set; }
        public string Type { get; set; }
        public TextContent Text { get; set; }
    }

    public class Delta
    {
        public List<Content> Content { get; set; } = new List<Content>();
    }

    public class DataDelta
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public Delta Delta { get; set; } = new Delta();
    }

    public class DataCompleted
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long CreatedAt { get; set; }
        public string AssistantId { get; set; }
        public string ThreadId { get; set; }
        public string RunId { get; set; }
        public string Status { get; set; }
        public string IncompleteDetails { get; set; }
        public long? IncompleteAt { get; set; }
        public long? CompletedAt { get; set; }
        public string Role { get; set; }
        public List<CompletedContent> Content { get; set; } = new List<CompletedContent>();
        public List<object> Attachments { get; set; } = new List<object>();
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    public class CompletedContent
    {
        public string Type { get; set; }
        public TextContent Text { get; set; }
    }

    public class ThreadEvent
    {
        public string Event { get; set; }
        public DataDelta DataDelta { get; set; } = new DataDelta();
        public DataCompleted DataCompleted { get; set; } = new DataCompleted();
    }
}