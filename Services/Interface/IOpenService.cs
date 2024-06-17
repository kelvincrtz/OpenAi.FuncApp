using System.Threading.Tasks;

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
        Task<string> GenerateCompletionAsync(string model, string prompt, int? maxTokens = null, double? temperature = null, double? topP = null, int? n = null, string[] stop = null);

        // Assistants
        Task<string> AssistAsync(string conversationId, string message);
    }
}