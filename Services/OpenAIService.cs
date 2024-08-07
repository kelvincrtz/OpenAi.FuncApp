using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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
                Assistant_Id = messageRequest.Assistant_Id,
                Model = messageRequest.Model
            }, newThread.Id);
        }

        public async Task<List<ThreadEventResponse>> ContinueThreadAsync(MessageRequest messageRequest, string threadId)
        {

            // 1. Add message to an existing thread
            await AddMessageAsync(messageRequest, threadId);

            // 2. Run thread
            return await CreateRun(new RunRequest
            {
                Assistant_Id = messageRequest.Assistant_Id,
                Model = messageRequest.Model
            }, threadId);
        }

        public async Task<QuestionsResponse> StartNewThreadJsonFormatAsync(MessageRequest messageRequest)
        {
            // 1. Create a new thread
            var newThread = await CreateNewThreadAsync();

            // 2. Add message
            await AddMessageAsync(messageRequest, newThread.Id);

            // 3. Run thread
            return await CreateRunJsonFormat(new RunRequest
            {
                Assistant_Id = messageRequest.Assistant_Id,
                Model = messageRequest.Model
            }, newThread.Id);
        }

        /// <summary>
        /// Threads
        /// </summary>
        public async Task<string> GetThreadMessagesAsync(string threadId)
        {
            return await GetAsync($"{_config.BaseUrl}/threads/{threadId}");
        }

        public async Task<ThreadResponse> CreateNewThreadAsync()
        {
            var jsonResponse = await PostAsync($"{_config.BaseUrl}/threads", null);
            return JsonConvert.DeserializeObject<ThreadResponse>(jsonResponse);
        }

        public async Task<string> ModifyThreadAsync(ThreadRequest threadRequest, string threadId)
        {
            return await PostAsync($"{_config.BaseUrl}/threads/{threadId}", threadRequest);
        }

        public async Task<string> DeleteThreadAsync(string threadId)
        {
            return await DeleteAsync($"{_config.BaseUrl}/threads/{threadId}");
        }

        /// <summary>
        /// Vectors store
        /// </summary>
        public async Task<VectorStore> CreateVectorStore()
        {
            var jsonResponse = await PostAsync($"{_config.BaseUrl}/vector_stores", null);
            return JsonConvert.DeserializeObject<VectorStore>(jsonResponse);
        }

        public async Task<VectorStoreListResponse> ListCreateVectorStores()
        {
            var jsonResponse = await GetAsync($"{_config.BaseUrl}/vector_stores");
            return JsonConvert.DeserializeObject<VectorStoreListResponse>(jsonResponse);
        }

        /// <summary>
        /// Vectors store files
        /// </summary>
        public async Task<VectorStoreFile> CreateVectorStoreFile(string vectorStoreId, byte[] fileData, string fileName)
        {
            using var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(fileData);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
            content.Add(fileContent, "file", fileName);

            var jsonResponse = await PostFileAsync($"{_config.BaseUrl}/vector_stores/{vectorStoreId}/files", content);
            return JsonConvert.DeserializeObject<VectorStoreFile>(jsonResponse);
        }

        public async Task<VectorStoreListResponse> ListVectorStoreFiles(string vectorStoreId)
        {
            var jsonResponse = await GetAsync($"{_config.BaseUrl}/vector_stores/{vectorStoreId}/files");
            return JsonConvert.DeserializeObject<VectorStoreListResponse>(jsonResponse);
        }

        public async Task<VectorStoreFile> RetrieveVectorStoreFile(string vectorStoreId, string fileId)
        {
            var jsonResponse = await GetAsync($"{_config.BaseUrl}/vector_stores/{vectorStoreId}/files/{fileId}");
            return JsonConvert.DeserializeObject<VectorStoreFile>(jsonResponse);
        }

        public async Task<VectorStoreFile> DeleteVectorStoreFile(string vectorStoreId, string fileId)
        {
            var jsonResponse = await DeleteAsync($"{_config.BaseUrl}/vector_stores/{vectorStoreId}/files/{fileId}");
            return JsonConvert.DeserializeObject<VectorStoreFile>(jsonResponse);
        }

        /// <summary>
        /// Classifications
        /// </summary>
        public async Task<string> ClassifyTextAsync(string text)
        {
            var requestBody = new
            {
                text
            };

            return await PostAsync($"{_config.BaseUrl}/classifications", requestBody);
        }

        /// <summary>
        /// Completions
        /// </summary>
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

        /// <summary>
        /// Assistants
        /// </summary>
        public async Task<Assistant> CreateAssistantAsync(AssistantRequest request)
        {
            var requestBody = new
            {
                instructions = request.Instructions,
                name = request.Name,
                tools = request.Tools,
                model = request.Model
            };

            var jsonResponse = await PostAsync($"{_config.BaseUrl}/assistants", requestBody);
            return JsonConvert.DeserializeObject<Assistant>(jsonResponse);
        }

        public async Task<AssistantListResponse> ListAssistantsAsync()
        {
            var jsonResponse = await GetAsync($"{_config.BaseUrl}/assistants");
            return JsonConvert.DeserializeObject<AssistantListResponse>(jsonResponse);
        }

        public async Task<Assistant> RetrieveAssistantAsync(string assistantId)
        {
            var jsonResponse = await GetAsync($"{_config.BaseUrl}/assistants/{assistantId}");
            return JsonConvert.DeserializeObject<Assistant>(jsonResponse);
        }

        public async Task<Assistant> ModifyAssistantAsync(AssistantRequest request, string assistantId)
        {
            var requestBody = new
            {
                instructions = request.Instructions,
                tools = request.Tools,
                model = request.Model
            };

            var jsonResponse = await PostAsync($"{_config.BaseUrl}/assistants/{assistantId}", requestBody);
            return JsonConvert.DeserializeObject<Assistant>(jsonResponse);
        }

        public async Task<Assistant> DeleteAssistantAsync(string assistantId)
        {
            var jsonResponse = await DeleteAsync($"{_config.BaseUrl}/assistants/{assistantId}");
            return JsonConvert.DeserializeObject<Assistant>(jsonResponse);
        }

        /// <summary>
        /// Messages
        /// </summary>
        public async Task<string> AddMessageAsync(MessageRequest messageRequest, string threadId)
        {
            var requestBody = new
            {
                role = messageRequest.Role,
                content = messageRequest.Content
            };

            return await PostAsync($"{_config.BaseUrl}/threads/{threadId}/messages", requestBody);
        }

        public async Task<MessageListResponse> ListMessagesAsync(string threadId)
        {
            var jsonResponse = await GetAsync($"{_config.BaseUrl}/threads/{threadId}/messages");
            return JsonConvert.DeserializeObject<MessageListResponse>(jsonResponse);
        }

        public async Task<Message> RetrieveMessagesAsync(string threadId, string messageId)
        {
            var jsonResponse = await GetAsync($"{_config.BaseUrl}/threads/{threadId}/messages/{messageId}");
            return JsonConvert.DeserializeObject<Message>(jsonResponse);
        }

        public async Task<string> ModifyMessagesAsync(MessageRequest messageRequest, string threadId, string messageId)
        {
            return await PostAsync($"{_config.BaseUrl}/threads/{threadId}/messages/{messageId}", messageRequest);
        }

        public async Task<string> DeleteMessagesAsync(string threadId, string messageId)
        {
            return await DeleteAsync($"{_config.BaseUrl}/threads/{threadId}/messages/{messageId}");
        }

        /// <summary>
        /// Runs
        /// </summary>
        public async Task<List<ThreadEventResponse>> CreateRun(RunRequest runRequest, string threadId)
        {
            var requestBody = new
            {
                assistant_id = runRequest.Assistant_Id,
                model = runRequest.Model,
                tool_choice = runRequest.Tool_Choice,
                stream = true
            };

            return await PostStreamAsync($"{_config.BaseUrl}/threads/{threadId}/runs", requestBody);
        }

        public async Task<QuestionsResponse> CreateRunJsonFormat(RunRequest runRequest, string threadId)
        {
            var requestBody = new
            {
                assistant_id = runRequest.Assistant_Id,
                model = runRequest.Model,
                tool_choice = runRequest.Tool_Choice,
                stream = true
            };

            return await PostStreamJsonFormatAsync($"{_config.BaseUrl}/threads/{threadId}/runs", requestBody);
        }

        /// <summary>
        /// Audio
        /// Create speech and create transcriptions
        /// </summary>
        public async Task<byte[]> CreateSpeechAsync(SpeechRequest request)
        {
            var requestBody = new
            {
                model = request.Model,
                input = request.Input,
                voice = request.Voice,
                response_format = request.ResponseFormat
            };

            return await PostByteAsync($"{_config.BaseUrl}/audio/speech", requestBody);
        }

        public async Task<TranscriptionResponse> CreateTranscriptionAsync(
            byte[] audioData,
            string fileName,
            string model = "whisper-1",
            string language = "en",
            string prompt = "",
            string responseFormat = "json")
        {
            using var content = new MultipartFormDataContent();
            var audioContent = new ByteArrayContent(audioData);
            audioContent.Headers.ContentType = new MediaTypeHeaderValue("audio/m4a"); // Adjust this to your audio file type
            content.Add(audioContent, "file", fileName);

            content.Add(new StringContent(model), "model");
            content.Add(new StringContent(language), "language");
            content.Add(new StringContent(prompt), "prompt");
            content.Add(new StringContent(responseFormat), "response_format");

            var jsonResponse = await PostFileAsync($"{_config.BaseUrl}/audio/transcriptions", content);
            return JsonConvert.DeserializeObject<TranscriptionResponse>(jsonResponse);
        }

        /// <summary>
        /// Helpers
        /// </summary>
        private async Task<string> PostAsync(string url, object requestBody)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonString, Encoding.UTF8, Defaults.JsonMediaType);
                HttpResponseMessage response = await PostHelperAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                dynamic responseData = JsonConvert.DeserializeObject(responseString);
                return JsonConvert.SerializeObject(responseData);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error while posting data from {url}: {ex.Message}", ex);
            }
        }

        // Post with streaming
        private async Task<List<ThreadEventResponse>> PostStreamAsync(string url, object requestBody)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonString, Encoding.UTF8, Defaults.JsonMediaType);
                HttpResponseMessage response = await PostHelperAsync(url, content);
                return await ProcessStreamingResponse(response);
            }
            catch (HttpRequestException httpEx)
            {
                throw new HttpRequestException($"HTTP request error while getting data from {url}: {httpEx.Message}", httpEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error while posting data from {url}: {ex.Message}", ex);
            }
        }

        // Post with streaming
        // Quetions Response
        private async Task<QuestionsResponse> PostStreamJsonFormatAsync(string url, object requestBody)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonString, Encoding.UTF8, Defaults.JsonMediaType);
                HttpResponseMessage response = await PostHelperAsync(url, content);
                return await ProcessStreamingJsonFormatResponse(response);
            }
            catch (HttpRequestException httpEx)
            {
                throw new HttpRequestException($"HTTP request error while getting data from {url}: {httpEx.Message}", httpEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error while posting data from {url}: {ex.Message}", ex);
            }
        }

        private async Task<string> PostFileAsync(string url, MultipartFormDataContent request)
        {
            try
            {
                DefaultHeaderHelper();
                var response = await _httpClient.PostAsync(url, request);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();

                dynamic responseData = JsonConvert.DeserializeObject(responseString);
                return JsonConvert.SerializeObject(responseData);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error while posting data from {url}: {ex.Message}", ex);
            }
        }

        // Post with byte[] return
        private async Task<byte[]> PostByteAsync(string url, object requestBody)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonString, Encoding.UTF8, Defaults.JsonMediaType);
                HttpResponseMessage response = await PostHelperAsync(url, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error while posting data from {url}: {ex.Message}", ex);
            }
        }

        private async Task<HttpResponseMessage> PostHelperAsync(string url, StringContent content)
        {
            DefaultHeaderHelper();
            var response = await _httpClient.PostAsync(url, content);
            return response;
        }

        private async Task<string> GetAsync(string url)
        {
            try
            {
                DefaultHeaderHelper();
                var response = await _httpClient.GetAsync(url);
                var responseString = await response.Content.ReadAsStringAsync();

                dynamic responseData = JsonConvert.DeserializeObject(responseString);
                return JsonConvert.SerializeObject(responseData);
            }
            catch (HttpRequestException httpEx)
            {
                throw new HttpRequestException($"HTTP request error while getting data from {url}: {httpEx.Message}", httpEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error while getting data from {url}: {ex.Message}", ex);
            }
        }

        private async Task<string> DeleteAsync(string url)
        {
            try
            {
                DefaultHeaderHelper();
                var response = await _httpClient.DeleteAsync(url);
                var responseString = await response.Content.ReadAsStringAsync();

                dynamic responseData = JsonConvert.DeserializeObject(responseString);
                return JsonConvert.SerializeObject(responseData);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error while deleting data from {url}: {ex.Message}", ex);
            }
        }

        private void DefaultHeaderHelper()
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add(Defaults.Authorization, $"Bearer {_config.ApiKey}");
            _httpClient.DefaultRequestHeaders.Add(Defaults.OpenAI_Beta, Defaults.AssistantsV2);
        }

        /// <summary>
        /// For regular responses
        /// Used for non-json formatted responses
        /// </summary>
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

        /// <summary>
        /// Used for Json formatted responses
        /// Mainly used to create quesionaires
        /// Related to business logic for SkillsVR
        /// </summary>
        private static async Task<QuestionsResponse> ProcessStreamingJsonFormatResponse(HttpResponseMessage response)
        {
            var responseDto = new QuestionsResponse();
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(responseStream))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    Console.Write(line);

                    if (line.StartsWith("event: thread.message.completed"))
                    {
                        var eventType = line.Substring(7); // Remove "event: " prefix
                        var dataLine = await reader.ReadLineAsync();
                        if (dataLine != null && dataLine.StartsWith("data: "))
                        {
                            var jsonData = dataLine.Substring(6); // Remove "data: " prefix
                            var threadEventResponse = new ThreadEventResponse
                            {
                                Event = eventType,
                                DataCompleted = JsonConvert.DeserializeObject<DataCompleted>(jsonData)
                            };

                            responseDto = QuestionsLogCompletedContent(threadEventResponse.DataCompleted.Content);
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
                Console.WriteLine("Content is null.");
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

        private static QuestionsResponse QuestionsLogCompletedContent(List<Content> contentList)
        {
            Console.WriteLine("Content:");
            var responseDto = new QuestionsResponse();
            if (contentList == null)
            {
                Console.WriteLine("Completed Content is null.");
                return null;
            }

            foreach (var content in contentList)
            {
                if (content?.Text != null)
                {
                    responseDto = JsonConvert.DeserializeObject<QuestionsResponse>(content.Text.Value);
                    Console.WriteLine($"Completed Content: {content.Text.Value}");
                }
                else
                {
                    Console.WriteLine("Completed Content is null.");
                }
            }

            return responseDto;
        }
    }
}
