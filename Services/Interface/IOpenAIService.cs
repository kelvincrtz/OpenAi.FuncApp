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

        // Runs (streaming response type)
        Task<List<ThreadEventResponse>> CreateRun(RunRequest runRequest, string threadId);
        Task<QuestionsResponse> CreateRunJsonFormat(RunRequest runRequest, string threadId);

        // Classifications
        Task<string> ClassifyTextAsync(string text);

        // Vector store
        Task<VectorStore> CreateVectorStore();
        Task<VectorStoreListResponse> ListCreateVectorStores();

        // Vectore store files
        Task<VectorStoreFile> CreateVectorStoreFile(string vectorStoreId, byte[] fileData, string fileName);
        Task<VectorStoreListResponse> ListVectorStoreFiles(string vectorStoreId);
        Task<VectorStoreFile> RetrieveVectorStoreFile(string vectorStoreId, string fileId);
        Task<VectorStoreFile> DeleteVectorStoreFile(string vectorStoreId, string fileId);

        // Completions
        Task<string> GenerateCompletionAsync(CompletionRequest request);

        // Assistants
        Task<Assistant> CreateAssistantAsync(AssistantRequest request);
        Task<AssistantListResponse> ListAssistantsAsync();
        Task<Assistant> RetrieveAssistantAsync(string assistantId);
        Task<Assistant> ModifyAssistantAsync(AssistantRequest request, string assistantId);
        Task<Assistant> DeleteAssistantAsync(string assistantId);

        // Audio - speech and transcriptions
        Task<byte[]> CreateSpeechAsync(SpeechRequest request);
        Task<TranscriptionResponse> CreateTranscriptionAsync(byte[] audioData, string fileName, string model = "whisper-1", string language = "en", string prompt = "", string responseFormat = "json");
    }
}