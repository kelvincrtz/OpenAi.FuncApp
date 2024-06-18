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
    public class AssistantsFunction
    {
        private readonly IOpenAIService _openAIService;

        public AssistantsFunction(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        [FunctionName("CreateAssistant")]
        public async Task<IActionResult> CreateAssistant(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "assistants")]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<AssistantRequest>(requestBody);

            if (string.IsNullOrEmpty(request.Instructions) || string.IsNullOrEmpty(request.Name))
            {
                return new BadRequestObjectResult("Instructions and name are required.");
            }

            try
            {
                string response = await _openAIService.CreateAssistantAsync(request);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error assisting.");
                return new StatusCodeResult(500);
            }
        }

        [FunctionName("ListAssistants")]
        public async Task<IActionResult> ListAssistants(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "assistants")]
            HttpRequest req,
            ILogger log)
        {
            try
            {
                string response = await _openAIService.ListAssistantsAsync();
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error getting the list of assistants.");
                return new StatusCodeResult(500);
            }
        }

        [FunctionName("RetrieveAssistant")]
        public async Task<IActionResult> RetrieveAssistant(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "assistants/{assistantId}")]
            HttpRequest req,
            string assistantId,
            ILogger log)
        {
            try
            {
                string response = await _openAIService.RetrieveAssistantAsync(assistantId);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error retrieving the assistant.");
                return new StatusCodeResult(500);
            }
        }

        [FunctionName("DeleteAssistant")]
        public async Task<IActionResult> DeleteAssistant(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "assistants/{assistantId}")]
            HttpRequest req,
            string assistantId,
            ILogger log)
        {
            try
            {
                string response = await _openAIService.DeleteAssistantAsync(assistantId);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error deleting the assistant.");
                return new StatusCodeResult(500);
            }
        }
    }
}