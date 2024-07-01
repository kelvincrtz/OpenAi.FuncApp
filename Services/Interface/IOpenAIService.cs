using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAi.FuncApp.Data.Requests;
using OpenAi.FuncApp.Data.Response;

namespace OpenAi.FuncApp.Services.Interface
{
    public interface IOpenAIService
    {
        // Main
        Task<List<ThreadEventResponse>> StartNewThreadAsync(MessageRequest messageRequest);
        Task<List<ThreadEventResponse>> ContinueThreadAsync(MessageRequest messageRequest, string threadId);
        Task<QuestionsResponse> StartNewThreadJsonFormatAsync(MessageRequest messageRequest);

        // Threads
        Task<ThreadResponse> CreateNewThreadAsync();
        Task<string> GetThreadMessagesAsync(string threadId);
        Task<string> ModifyThreadAsync(ThreadRequest threadRequest, string threadId);
        Task<string> DeleteThreadAsync(string threadId);

        // Messages
        Task<string> AddMessageAsync(MessageRequest messageRequest, string threadId);
        Task<MessageListResponse> ListMessagesAsync(string threadId);
        Task<Message> RetrieveMessagesAsync(string threadId, string messageId);
        Task<string> ModifyMessagesAsync(MessageRequest messageRequest, string threadId, string messageId);
        Task<string> DeleteMessagesAsync(string threadId, string messageId);

        // Runs (Streaming Response type)
        Task<List<ThreadEventResponse>> CreateRun(RunRequest runRequest, string threadId);
        Task<QuestionsResponse> CreateRunJsonFormat(RunRequest runRequest, string threadId);

        // Classifications
        Task<string> ClassifyTextAsync(string text);

        // Vectors and Files
        Task<VectorStore> CreateVectorStore();
        Task<VectorStoreListResponse> ListCreateVectorStores();
        Task<VectorStore> CreateVectorStoreFile(string vectorStoreId);
        Task<VectorStoreListResponse> ListVectorStoreFiles(string vectorStoreId);

        // Completions
        Task<string> GenerateCompletionAsync(CompletionRequest request);

        // Assistants
        Task<string> CreateAssistantAsync(AssistantRequest request);
        Task<string> ListAssistantsAsync();
        Task<string> RetrieveAssistantAsync(string assistantId);
        Task<string> ModifyAssistantAsync(AssistantRequest request, string assistantId);
        Task<string> DeleteAssistantAsync(string assistantId);

        // Audio - Create speech and create transcriptions
        Task<object> CreateSpeechAsync(SpeechRequest request);
        Task<object> CreateTranscriptionAsync(
            byte[] audioData,
            string fileName,
            string model = "whisper-1",
            string language = "en",
            string prompt = "",
            string responseFormat = "json");
    }
}