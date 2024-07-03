using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenAi.FuncApp.Data.Requests;
using OpenAi.FuncApp.Services.Interface;

namespace OpenAi.FuncApp.Functions
{
    [ApiExplorerSettings(GroupName = "Completions")]
    public class CompletionsFunction
    {
        /// <summary>
        /// Completion APIs will soon be a legacy product of OpenAI
        /// Another option is to use Threads
        /// </summary>
        private readonly IOpenAIService _openAIService;

        /// <summary>
        /// </summary>
        /// <param name="openAIService"></param>
        public CompletionsFunction(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        [FunctionName("GenerateCompletion")]
        public async Task<IActionResult> Generate(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "completions")]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<CompletionRequest>(requestBody);

            if (string.IsNullOrEmpty(request.Model))
            {
                return new BadRequestObjectResult("Model is required.");
            }

            try
            {
                string response = await _openAIService.GenerateCompletionAsync(request);
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