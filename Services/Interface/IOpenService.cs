using System.Threading.Tasks;
using OpenAi.FuncApp.Data.Requests;

namespace OpenAi.FuncApp.Services.Interface
{
    public interface IOpenAIService
    {
        // Threads
        Task<string> GetThreadMessagesAsync(string threadId);
        Task<string> StartNewThreadAsync(string initialMessage);
        Task<string> ContinueThreadAsync(string threadId, string message);

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