using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace OpenAI.FuncApp.Services
{
    public class ThreadService
    {
        private readonly OpenAIService _openAIService;

        public ThreadService(OpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        public async Task<string> StartNewThread(string initialMessage)
        {
            var requestBody = new
            {
                model = _openAIService.Model,
                messages = new[]
                {
                    new { role = "user", content = initialMessage }
                }
            };

            var response = await _openAIService.PostAsync("https://api.openai.com/v1/threads", requestBody);
            var jsonResponse = JObject.Parse(response);
            var threadId = jsonResponse["id"]?.ToString();
            var assistantMessage = jsonResponse["messages"]?[1]?["content"]?.ToString();

            Console.WriteLine($"Assistant: {assistantMessage}");

            return threadId;
        }

        public async Task ContinueThread(string threadId, string newMessage)
        {
            var requestBody = new
            {
                messages = new[]
                {
                    new { role = "user", content = newMessage }
                }
            };

            await _openAIService.PostAsync($"https://api.openai.com/v1/threads/{threadId}/runs", requestBody);
        }
    }
}