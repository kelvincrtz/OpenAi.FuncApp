using System;
using System.IO;
using System.Linq;
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
    public class ThreadsFunction
    {
        private readonly IOpenAIService _openAIService;

        public ThreadsFunction(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        [FunctionName("HandleThread")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "threads/{action}/{threadId?}")]
            HttpRequest req,
            string action,
            string threadId,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<CompletionRequest>(requestBody);

            try
            {
                string response;
                switch (action.ToLower())
                {
                    case "start":
                        if (request.Messages.Count == 0 || string.IsNullOrEmpty(request.Model))
                        {
                            return new BadRequestObjectResult("Initial message and model are required.");
                        }
                        response = await _openAIService.StartNewThreadAsync(request);
                        break;

                    case "continue":
                        if (request.Messages.Count == 0 || string.IsNullOrEmpty(threadId))
                        {
                            return new BadRequestObjectResult("Thread Id and message are required.");
                        }
                        request.ThreadId = threadId;
                        response = await _openAIService.ContinueThreadAsync(request, threadId);
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