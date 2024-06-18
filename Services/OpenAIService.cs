using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenAi.FuncApp.Configuration;
using OpenAi.FuncApp.Data.Requests;
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

        // Threads
        public async Task<string> GetThreadMessagesAsync(string threadId)
        {
            return await GetAsync($"{_config.BaseUrl}/threads/{threadId}");
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

        // Vectors
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

        // Classifications
        public async Task<string> ClassifyTextAsync(string text)
        {
            var requestBody = new
            {
                text
            };

            return await PostAsync($"{_config.BaseUrl}/classifications", requestBody);
        }

        // Completions
        public async Task<string> GenerateCompletionAsync(CompletionRequest request)
        {
            var requestBody = new
            {
                model = request.Model,
                prompt = request.Prompt,
                max_tokens = request.MaxTokens,
                temperature = request.Temperature,
                messages = request.Messages
            };

            return await PostAsync($"{_config.BaseUrl}/chat/completions", requestBody);
        }

        // Assistant
        public async Task<string> CreateAssistantAsync(AssistantRequest request)
        {
            var requestBody = new
            {
                instructions = request.Instructions,
                name = request.Name,
                tools = request.Tools,
                model = request.Model
            };

            return await AssistantPostAsync($"{_config.BaseUrl}/assistants", requestBody);
        }

        public async Task<string> ListAssistantsAsync()
        {
            return await AssistantGetAsync($"{_config.BaseUrl}/assistants");
        }

        public async Task<string> RetrieveAssistantAsync(string assistantId)
        {
            return await AssistantGetAsync($"{_config.BaseUrl}/assistants/{assistantId}");
        }

        public async Task<string> ModifyAssistantAsync(AssistantRequest request, string assistantId)
        {
            var requestBody = new
            {
                instructions = request.Instructions,
                tools = request.Tools,
                model = request.Model
            };

            return await AssistantPostAsync($"{_config.BaseUrl}/assistants/{assistantId}", requestBody);
        }

        public async Task<string> DeleteAssistantAsync(string assistantId)
        {
            return await AssistantDeleteAsync($"{_config.BaseUrl}/assistants/{assistantId}");
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

        private async Task<string> GetAsync(string url)
        {
            try
            {

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");
                var response = await _httpClient.GetAsync(url);
                var responseString = await response.Content.ReadAsStringAsync();

                dynamic responseData = JsonConvert.DeserializeObject(responseString);
                return JsonConvert.SerializeObject(responseData);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error posting to {url}: {ex.Message}");
            }
        }

        // Need a different PostAsync for the Assistant APIs
        private async Task<string> AssistantPostAsync(string url, object requestBody)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");
                _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", $"assistants=v2");
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

        private async Task<string> AssistantGetAsync(string url)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");
                _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", $"assistants=v2");
                var response = await _httpClient.GetAsync(url);
                var responseString = await response.Content.ReadAsStringAsync();

                dynamic responseData = JsonConvert.DeserializeObject(responseString);
                return JsonConvert.SerializeObject(responseData);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting to {url}: {ex.Message}");
            }
        }

        private async Task<string> AssistantDeleteAsync(string url)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");
                _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", $"assistants=v2");
                var response = await _httpClient.DeleteAsync(url);
                var responseString = await response.Content.ReadAsStringAsync();

                dynamic responseData = JsonConvert.DeserializeObject(responseString);
                return JsonConvert.SerializeObject(responseData);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting to {url}: {ex.Message}");
            }
        }
    }
}
