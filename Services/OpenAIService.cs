using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenAi.FuncApp.Configuration;
using OpenAi.FuncApp.Constants;
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
            return await V2GetAsync($"{_config.BaseUrl}/threads/{threadId}");
        }

        public async Task<string> StartNewThreadAsync(string initialMessage)
        {
            var requestBody = new
            {
                messages = new[]
                {
                new
                {
                    role = "user",
                    content = initialMessage
                }
            }
            };
            return await V2PostAsync($"{_config.BaseUrl}/threads", requestBody);
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
            return await V2PostAsync($"{_config.BaseUrl}/threads/{threadId}/messages", requestBody);
        }

        public async Task<string> ModifyThreadAsync(ThreadRequest threadRequest, string threadId)
        {
            return await V2PostAsync($"{_config.BaseUrl}/threads/{threadId}", threadRequest);
        }

        public async Task<string> DeleteThreadAsync(string threadId)
        {
            return await V2DeleteAsync($"{_config.BaseUrl}/threads/{threadId}");
        }

        // Vectors
        public async Task<string> SearchVectorStoreAsync(string query)
        {
            var requestBody = new
            {
                query
            };

            return await V1PostAsync($"{_config.BaseUrl}/vector-store/search", requestBody);
        }

        public async Task<string> InsertVectorAsync(string vectorData)
        {
            var requestBody = new
            {
                vectorData
            };

            return await V1PostAsync($"{_config.BaseUrl}/vector-store/insert", requestBody);
        }

        // Classifications
        public async Task<string> ClassifyTextAsync(string text)
        {
            var requestBody = new
            {
                text
            };

            return await V1PostAsync($"{_config.BaseUrl}/classifications", requestBody);
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

            return await V1PostAsync($"{_config.BaseUrl}/chat/completions", requestBody);
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

            return await V2PostAsync($"{_config.BaseUrl}/assistants", requestBody);
        }

        public async Task<string> ListAssistantsAsync()
        {
            return await V2GetAsync($"{_config.BaseUrl}/assistants");
        }

        public async Task<string> RetrieveAssistantAsync(string assistantId)
        {
            return await V2GetAsync($"{_config.BaseUrl}/assistants/{assistantId}");
        }

        public async Task<string> ModifyAssistantAsync(AssistantRequest request, string assistantId)
        {
            var requestBody = new
            {
                instructions = request.Instructions,
                tools = request.Tools,
                model = request.Model
            };

            return await V2PostAsync($"{_config.BaseUrl}/assistants/{assistantId}", requestBody);
        }

        public async Task<string> DeleteAssistantAsync(string assistantId)
        {
            return await V2DeleteAsync($"{_config.BaseUrl}/assistants/{assistantId}");
        }

        // Messages
        public async Task<string> CreateMessageAsync(MessageRequest messageRequest, string threadId)
        {
            var requestBody = new
            {
                role = messageRequest.Role,
                content = messageRequest.Content
            };

            return await V2PostAsync($"{_config.BaseUrl}/threads/{threadId}/messages", requestBody);
        }

        public async Task<string> ListMessagesAsync(string threadId)
        {
            return await V2GetAsync($"{_config.BaseUrl}/threads/{threadId}/messages");
        }

        public async Task<string> RetrieveMessagesAsync(string threadId, string messageId)
        {
            return await V2GetAsync($"{_config.BaseUrl}/threads/{threadId}/messages/{messageId}");
        }

        // Helpers
        private async Task<string> V1PostAsync(string url, object requestBody)
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

        private async Task<string> V1GetAsync(string url)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add(Defaults.Authorization, $"Bearer {_config.ApiKey}");
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

        private async Task<string> V2PostAsync(string url, object requestBody)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add(Defaults.Authorization, $"Bearer {_config.ApiKey}");
                _httpClient.DefaultRequestHeaders.Add(Defaults.OpenAI_Beta, $"assistants=v2");
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

        private async Task<string> V2GetAsync(string url)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add(Defaults.Authorization, $"Bearer {_config.ApiKey}");
                _httpClient.DefaultRequestHeaders.Add(Defaults.OpenAI_Beta, $"assistants=v2");
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

        private async Task<string> V2DeleteAsync(string url)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add(Defaults.Authorization, $"Bearer {_config.ApiKey}");
                _httpClient.DefaultRequestHeaders.Add(Defaults.OpenAI_Beta, $"assistants=v2");
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
