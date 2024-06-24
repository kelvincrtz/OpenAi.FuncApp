using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using OpenAi.FuncApp.Services.Interface;

namespace OpenAi.FuncApp.Services
{

    public class MessagesFunction
    {
        private readonly IOpenAIService _openAIService;

        public MessagesFunction(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        [FunctionName("ListMessages")]
        public async Task<IActionResult> ListMessages(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "messages/{threadId}")]
            HttpRequest req,
            string threadId,
            ILogger log)
        {
            try
            {
                return new OkObjectResult(await _openAIService.ListMessagesAsync(threadId));
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error getting the list of messages.");
                return new StatusCodeResult(500);
            }
        }

        [FunctionName("RetrieveMessage")]
        public async Task<IActionResult> RetrieveMessage(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "messages/{messageId}/threads/{threadId}")]
            HttpRequest req,
            string threadId,
            string messageId,
            ILogger log)
        {
            try
            {
                return new OkObjectResult(await _openAIService.RetrieveMessagesAsync(threadId, messageId));
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error getting the list of messages.");
                return new StatusCodeResult(500);
            }
        }
    }
}