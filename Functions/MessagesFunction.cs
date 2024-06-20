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

namespace OpenAi.FuncApp.Services
{
    public class MessagesFunction
    {
        private readonly IOpenAIService _openAIService;

        public MessagesFunction(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        [FunctionName("AddMessage")]
        public async Task<IActionResult> AddMessage(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "messages/add/{threadId}")]
            HttpRequest req,
            string threadId,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<MessageRequest>(requestBody);

            try
            {
                string response = await _openAIService.AddMessageAsync(request, threadId);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error getting the list of messages.");
                return new StatusCodeResult(500);
            }
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
                string response = await _openAIService.ListMessagesAsync(threadId);
                return new OkObjectResult(response);
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
                string response = await _openAIService.RetrieveMessagesAsync(threadId, messageId);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error getting the list of messages.");
                return new StatusCodeResult(500);
            }
        }
    }
}