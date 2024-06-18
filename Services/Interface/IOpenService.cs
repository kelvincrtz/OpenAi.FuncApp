using System.Threading.Tasks;
using OpenAi.FuncApp.Data.Requests;

namespace OpenAi.FuncApp.Services.Interface
{
    public interface IOpenAIService
    {
        // Threads
        Task<string> GetThreadMessagesAsync(string threadId);
        Task<string> StartNewThreadAsync(CompletionRequest request);
        Task<string> ContinueThreadAsync(CompletionRequest request, string threadId);
        Task<string> ModifyThreadAsync(ThreadRequest threadRequest, string threadId);
        Task<string> DeleteThreadAsync(string threadId);

        // Messages
        Task<string> CreateMessageAsync(MessageRequest messageRequest, string threadId);
        Task<string> ListMessagesAsync(string threadId);
        Task<string> RetrieveMessagesAsync(string threadId, string messageId);
        Task<string> ModifyMessagesAsync(MessageRequest messageRequest, string threadId, string messageId);
        Task<string> DeleteMessagesAsync(string threadId, string messageId);

        // Classifications
        Task<string> ClassifyTextAsync(string text);

        // Vectors
        Task<string> SearchVectorStoreAsync(string query);
        Task<string> InsertVectorAsync(string vectorData);

        // Completions
        Task<string> GenerateCompletionAsync(CompletionRequest request);

        // Assistants
        Task<string> CreateAssistantAsync(AssistantRequest request);
        Task<string> ListAssistantsAsync();
        Task<string> RetrieveAssistantAsync(string assistantId);
        Task<string> ModifyAssistantAsync(AssistantRequest request, string assistantId);
        Task<string> DeleteAssistantAsync(string assistantId);
    }
}