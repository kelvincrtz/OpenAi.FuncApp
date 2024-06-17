using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAI.FuncApp.Services;

namespace OpenAi.FuncApp.Services
{
    public class ClassificationService
    {
        private readonly OpenAIService _openAIService;

        public ClassificationService(OpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        public async Task<string> ClassifyTextAsync(string text, string model)
        {
            var requestData = new
            {
                model = model,
                examples = new[]
                {
                    new { label = "Label 1", text = "Example text 1" },
                    new { label = "Label 2", text = "Example text 2" }
                    // Add more labeled examples as needed
                },
                query = text
            };

            var jsonRequest = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _openAIService.PostAsync("https://api.openai.com/v1/classifications", content);
            return response;
        }
    }
}