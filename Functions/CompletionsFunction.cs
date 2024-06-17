using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenAi.FuncApp.Services.Interface;

namespace OpenAi.FuncApp.Functions
{
    public class CompletionsFunction
    {
        private readonly IOpenAIService _openAIService;

        public CompletionsFunction(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        [FunctionName("GenerateCompletion")]
        public async Task<IActionResult> Generate(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "completions")] HttpRequestMessage req,
            ILogger log)
        {
            string requestBody = await req.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string model = data?.model;
            string prompt = data?.prompt;
            int? maxTokens = data?.max_tokens;
            double? temperature = data?.temperature;
            double? topP = data?.top_p;
            int? n = data?.n;
            string[] stop = data?.stop?.ToObject<string[]>();

            if (string.IsNullOrEmpty(model) || string.IsNullOrEmpty(prompt))
            {
                return new BadRequestObjectResult("Model and prompt are required.");
            }

            try
            {
                string response = await _openAIService.GenerateCompletionAsync(model, prompt, maxTokens, temperature, topP, n, stop);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error generating completion.");
                return new StatusCodeResult(500);
            }
        }
    }
}