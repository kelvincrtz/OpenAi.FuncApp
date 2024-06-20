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

    public class Data
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public Delta Delta { get; set; } = new Delta();
    }

    public class ThreadEvent
    {
        public string Event { get; set; }
        public Data Data { get; set; } = new Data();
    }
}