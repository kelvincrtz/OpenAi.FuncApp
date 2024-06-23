using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenAi.FuncApp.Configuration;
using OpenAi.FuncApp.Constants;
using OpenAi.FuncApp.Data.Requests;
using OpenAi.FuncApp.Data.Response;
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

        public async Task<List<ThreadEventResponse>> StartNewThreadAsync(MessageRequest messageRequest)
        {
            // 1. Create a new thread
            var newThread = await CreateNewThreadAsync();

            // 2. Add message
            await AddMessageAsync(messageRequest, newThread.Id);

            // 3. Run thread
            return await CreateRun(new RunRequest
            {
                Assistant_Id = messageRequest.Assistant_Id
            }, newThread.Id);
        }

        public async Task<List<ThreadEventResponse>> ContinueThreadAsync(MessageRequest messageRequest, string threadId)
        {

            // 1. Add message to an existing thread
            await AddMessageAsync(messageRequest, threadId);

            // 2. Run thread
            return await CreateRun(new RunRequest
            {
                Assistant_Id = messageRequest.Assistant_Id
            }, threadId);
        }

        // Threads
        public async Task<string> GetThreadMessagesAsync(string threadId)
        {
            return await V2GetAsync($"{_config.BaseUrl}/threads/{threadId}");
        }

        public async Task<ThreadResponse> CreateNewThreadAsync()
        {
            var jsonResponse = await V2PostAsync($"{_config.BaseUrl}/threads", null);
            return JsonConvert.DeserializeObject<ThreadResponse>(jsonResponse);
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
        public async Task<string> AddMessageAsync(MessageRequest messageRequest, string threadId)
        {
            var requestBody = new
            {
                role = messageRequest.Role,
                content = messageRequest.Content // TODO: for the time being we only use a Text type for Content. We might need more options later
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

        public async Task<string> ModifyMessagesAsync(MessageRequest messageRequest, string threadId, string messageId)
        {
            return await V2PostAsync($"{_config.BaseUrl}/threads/{threadId}/messages/{messageId}", messageRequest);
        }

        public async Task<string> DeleteMessagesAsync(string threadId, string messageId)
        {
            return await V2DeleteAsync($"{_config.BaseUrl}/threads/{threadId}/messages/{messageId}");
        }

        // Runs
        public async Task<List<ThreadEventResponse>> CreateRun(RunRequest runRequest, string threadId)
        {
            // TODO: maybe do not need to do another mapping here if json works okay with runRequest
            var requestBody = new
            {
                assistant_id = runRequest.Assistant_Id,
                additional_instructions = runRequest.Additional_Instructions,
                tool_choice = runRequest.Tool_Choice,
                stream = true
            };

            return await V2PostStreamAsync($"{_config.BaseUrl}/threads/{threadId}/runs", requestBody);
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

        private async Task<List<ThreadEventResponse>> V2PostStreamAsync(string url, object requestBody)
        {
            try
            {
                var responseDto = new List<ThreadEventResponse>();
                var jsonString = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add(Defaults.Authorization, $"Bearer {_config.ApiKey}");
                _httpClient.DefaultRequestHeaders.Add(Defaults.OpenAI_Beta, $"assistants=v2");
                var response = await _httpClient.PostAsync(url, content);
                try
                {
                    responseDto = await ProcessStreamingResponse(response);
                }
                catch (HttpRequestException httpEx)
                {
                    Console.WriteLine($"Request Error: {httpEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected Error: {ex.Message}");
                }

                return responseDto;
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

        private static async Task<List<ThreadEventResponse>> ProcessStreamingResponse(HttpResponseMessage response)
        {
            var responseDto = new List<ThreadEventResponse>();

            using (var responseStream = await response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(responseStream))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();

                    if (line != null && (line.StartsWith("event: thread.message.delta") || line.StartsWith("event: thread.message.completed")))
                    {
                        var eventType = line.Substring(7); // Remove "event: " prefix
                        var dataLine = await reader.ReadLineAsync();
                        if (dataLine != null && dataLine.StartsWith("data: "))
                        {
                            var jsonData = dataLine.Substring(6); // Remove "data: " prefix
                            var threadEventResponse = new ThreadEventResponse { Event = eventType };

                            if (eventType == "thread.message.delta")
                            {
                                threadEventResponse.DataDelta = JsonConvert.DeserializeObject<DataDelta>(jsonData);
                            }
                            else if (eventType == "thread.message.completed")
                            {
                                threadEventResponse.DataCompleted = JsonConvert.DeserializeObject<DataCompleted>(jsonData);
                            }

                            responseDto.Add(HandleEvent(threadEventResponse));
                        }
                    }
                }
            }

            return responseDto;
        }

        private static ThreadEventResponse HandleEvent(ThreadEventResponse threadEvent)
        {
            if (threadEvent == null)
            {
                Console.WriteLine("ThreadEvent is null.");
                return null;
            }

            Console.WriteLine($"Event: {threadEvent.Event}");

            switch (threadEvent.Event)
            {
                case "thread.message.delta":
                    HandleDeltaEvent(threadEvent.DataDelta);
                    break;
                case "thread.message.completed":
                    HandleCompletedEvent(threadEvent.DataCompleted);
                    break;
                default:
                    Console.WriteLine("Unknown event type.");
                    break;
            }

            return threadEvent;
        }

        private static void HandleDeltaEvent(DataDelta dataDelta)
        {
            if (dataDelta?.Delta?.Content == null)
            {
                Console.WriteLine("DataDelta.Delta.Content is null.");
                return;
            }

            foreach (var content in dataDelta.Delta.Content)
            {
                LogContent(content);
            }
        }

        private static void HandleCompletedEvent(DataCompleted dataCompleted)
        {
            if (dataCompleted == null)
            {
                Console.WriteLine("DataCompleted is null.");
                return;
            }

            // LogCompletedDetails(dataCompleted); Not needed
            LogCompletedContent(dataCompleted.Content);
        }

        private static void LogContent(Content content)
        {
            if (content?.Text != null)
            {
                Console.WriteLine($"Delta Content: {content.Text.Value}");
            }
            else
            {
                Console.WriteLine("Delta Content is null.");
            }
        }

        private static void LogCompletedContent(List<Content> contentList)
        {
            Console.WriteLine("Content:");

            if (contentList == null)
            {
                Console.WriteLine("Completed Content is null.");
                return;
            }

            foreach (var content in contentList)
            {
                if (content?.Text != null)
                {
                    Console.WriteLine($"Completed Content: {content.Text.Value}");
                }
                else
                {
                    Console.WriteLine("Completed Content is null.");
                }
            }
        }

        // not used for now - for testing purposes only
        private static void LogCompletedDetails(DataCompleted dataCompleted)
        {
            Console.WriteLine($"ID: {dataCompleted.Id}");
            Console.WriteLine($"Object: {dataCompleted.Object}");
            Console.WriteLine($"Created At: {dataCompleted.CreatedAt}");
            Console.WriteLine($"Assistant ID: {dataCompleted.AssistantId}");
            Console.WriteLine($"Thread ID: {dataCompleted.ThreadId}");
            Console.WriteLine($"Run ID: {dataCompleted.RunId}");
            Console.WriteLine($"Status: {dataCompleted.Status}");
            Console.WriteLine($"Incomplete Details: {dataCompleted.IncompleteDetails}");
            Console.WriteLine($"Incomplete At: {dataCompleted.IncompleteAt}");
            Console.WriteLine($"Completed At: {dataCompleted.CompletedAt}");
            Console.WriteLine($"Role: {dataCompleted.Role}");
        }
    }
}
