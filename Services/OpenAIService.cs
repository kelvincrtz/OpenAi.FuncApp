using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenAi.FuncApp.Configuration;
using OpenAi.FuncApp.Services.Interface;

namespace OpenAI.FuncApp.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAIConfig _config;

        public OpenAIService(HttpClient httpClient, IOptions<OpenAIConfig> config)
        {
            _httpClient = httpClient;
            _config = config.Value;
        }

        public async Task<string> GetThreadMessagesAsync(string threadId)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");
                var response = await _httpClient.GetAsync($"{_config.BaseUrl}/threads/{threadId}");
                var responseString = await response.Content.ReadAsStringAsync();

                dynamic responseData = JsonConvert.DeserializeObject(responseString);
                return JsonConvert.SerializeObject(responseData);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving thread messages: {ex.Message}");
            }
        }

        public async Task<string> StartNewThreadAsync(string initialMessage)
        {
            var requestBody = new
            {
                messages = new[]
                {
                new
                {
                    role = "system",
                    content = initialMessage
                }
            }
            };
            return await PostAsync($"{_config.BaseUrl}/threads", requestBody);
        }

        public async Task<string> ContinueThreadAsync(string threadId, string message)
        {
            var requestBody = new
            {
                messages = new[]
                {
                new
                {
                    role = "user",
                    content = message
                }
            }
            };
            return await PostAsync($"{_config.BaseUrl}/threads/{threadId}/messages", requestBody);
        }

        public async Task<string> SearchVectorStoreAsync(string query)
        {
            var requestBody = new
            {
                query
            };

            return await PostAsync($"{_config.BaseUrl}/vector-store/search", requestBody);
        }

        public async Task<string> InsertVectorAsync(string vectorData)
        {
            var requestBody = new
            {
                vectorData
            };

            return await PostAsync($"{_config.BaseUrl}/vector-store/insert", requestBody);
        }

        public async Task<string> ClassifyTextAsync(string text)
        {
            var requestBody = new
            {
                text
            };

            return await PostAsync($"{_config.BaseUrl}/classifications", requestBody);
        }

        public async Task<string> GenerateCompletionAsync(string model, string prompt, int? maxTokens = null, double? temperature = null, double? topP = null, int? n = null, string[] stop = null)
        {
            var requestBody = new
            {
                model,
                prompt,
                max_tokens = maxTokens,
                temperature,
                top_p = topP,
                n,
                stop
            };

            return await PostAsync($"{_config.BaseUrl}/completions", requestBody);
        }

        public async Task<string> AssistAsync(string conversationId, string message)
        {
            var requestBody = new
            {
                conversation_id = conversationId,
                message
            };

            return await PostAsync($"{_config.BaseUrl}/assistants", requestBody);
        }

        private async Task<string> PostAsync(string url, object requestBody)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");
                var response = await _httpClient.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                dynamic responseData = JsonConvert.DeserializeObject(responseString);
                return JsonConvert.SerializeObject(responseData);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error posting to {url}: {ex.Message}");
            }
        }
    }
}
