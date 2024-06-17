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
    public class AssistantsFunction
    {
        private readonly IOpenAIService _openAIService;

        public AssistantsFunction(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        [FunctionName("Assist")]
        public async Task<IActionResult> Assist(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "assistants")] HttpRequestMessage req,
            ILogger log)
        {
            string requestBody = await req.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string conversationId = data?.conversationId;
            string message = data?.message;

            if (string.IsNullOrEmpty(conversationId) || string.IsNullOrEmpty(message))
            {
                return new BadRequestObjectResult("Conversation ID and message are required.");
            }

            try
            {
                string response = await _openAIService.AssistAsync(conversationId, message);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error assisting.");
                return new StatusCodeResult(500);
            }
        }
    }
}