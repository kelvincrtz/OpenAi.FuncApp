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
    public class ThreadsFunction
    {
        private readonly IOpenAIService _openAIService;

        public ThreadsFunction(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        [FunctionName("HandleThread")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "threads/{action}/{threadId?}")] HttpRequestMessage req,
            string action,
            string threadId,
            ILogger log)
        {
            string requestBody = await req.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            try
            {
                string response;
                switch (action.ToLower())
                {
                    case "start":
                        string initialMessage = data?.initialMessage;
                        if (string.IsNullOrEmpty(initialMessage))
                        {
                            return new BadRequestObjectResult("Initial message is required.");
                        }
                        response = await _openAIService.StartNewThreadAsync(initialMessage);
                        break;

                    case "continue":
                        string message = data?.message;
                        if (string.IsNullOrEmpty(threadId) || string.IsNullOrEmpty(message))
                        {
                            return new BadRequestObjectResult("Thread ID and message are required.");
                        }
                        response = await _openAIService.ContinueThreadAsync(threadId, message);
                        break;

                    default:
                        return new BadRequestObjectResult("Invalid action.");
                }

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Error handling thread action '{action}' for thread '{threadId}'.");
                return new StatusCodeResult(500);
            }
        }
    }
}