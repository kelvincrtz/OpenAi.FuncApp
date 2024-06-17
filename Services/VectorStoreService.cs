using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenAI.FuncApp.Configuration;

namespace MyVectorFunctionApp.Services
{
    public class VectorStoreService
    {
        private readonly HttpClient _httpClient;
        private readonly VectorStoreConfiguration _vectorStoreConfiguration;

        public VectorStoreService(HttpClient httpClient, VectorStoreConfiguration vectorStoreConfiguration)
        {
            _httpClient = httpClient;
            _vectorStoreConfiguration = vectorStoreConfiguration;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _vectorStoreConfiguration.VectorStoreApiKey);
        }

        public async Task InsertVectorAsync(string vectorId, double[] vectorData)
        {
            var request = new
            {
                Id = vectorId,
                Data = vectorData
            };

            var jsonRequest = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_vectorStoreConfiguration.VectorStoreApiUrl}/insert", content);
            response.EnsureSuccessStatusCode();
        }

        // Other methods for querying vectors, similarity search, etc.
    }
}

