using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenAi.FuncApp.Data.Response
{
    public class FileCounts
    {
        [JsonProperty("in_progress")]
        public int InProgress { get; set; }

        [JsonProperty("completed")]
        public int Completed { get; set; }

        [JsonProperty("failed")]
        public int Failed { get; set; }

        [JsonProperty("cancelled")]
        public int Cancelled { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }

    public class VectorStoreFile
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("created_at")]
        public long CreatedAt { get; set; }

        [JsonProperty("vector_store_id")]
        public string VectorStoreId { get; set; }

        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
    }

    public class VectorStore
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("created_at")]
        public long CreatedAt { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("bytes")]
        public int Bytes { get; set; }

        [JsonProperty("file_counts")]
        public FileCounts FileCounts { get; set; }
    }


    public class VectorStoreListResponse
    {
        [JsonProperty("data")]
        public List<VectorStore> Message { get; set; }
    }

}