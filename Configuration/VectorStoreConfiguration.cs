using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace OpenAI.FuncApp.Configuration
{
    public class VectorStoreConfiguration
    {
        public string VectorStoreApiUrl { get; }
        public string VectorStoreApiKey { get; }

        public VectorStoreConfiguration(IConfiguration configuration)
        {
            VectorStoreApiUrl = configuration["VectorStoreApiUrl"];
            VectorStoreApiKey = configuration["VectorStoreApiKey"];
        }
    }
}
