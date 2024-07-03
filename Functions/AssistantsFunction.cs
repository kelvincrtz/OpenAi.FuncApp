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
    [ApiExplorerSettings(GroupName = "Assistants")]
    public class AssistantsFunction
    {
        private readonly IOpenAIService _openAIService;

        /// <summary>
        /// </summary>
        /// <param name="openAIService"></param>        
        public AssistantsFunction(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        /// <summary>
        /// Create assistant
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
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
                return new OkObjectResult(await _openAIService.CreateAssistantAsync(request));
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error assisting.");
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// List assistants
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("ListAssistants")]
        public async Task<IActionResult> ListAssistants(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "assistants")]
            HttpRequest req,
            ILogger log)
        {
            try
            {
                return new OkObjectResult(await _openAIService.ListAssistantsAsync());
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error getting the list of assistants.");
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Retrieve assistant by id
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("RetrieveAssistant")]
        public async Task<IActionResult> RetrieveAssistant(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "assistants/{assistantId}")]
            HttpRequest req,
            string assistantId,
            ILogger log)
        {
            try
            {
                return new OkObjectResult(await _openAIService.RetrieveAssistantAsync(assistantId));
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error retrieving the assistant.");
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Delete assistant
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("DeleteAssistant")]
        public async Task<IActionResult> DeleteAssistant(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "assistants/{assistantId}")]
            HttpRequest req,
            string assistantId,
            ILogger log)
        {
            try
            {
                return new OkObjectResult(await _openAIService.DeleteAssistantAsync(assistantId));
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error deleting the assistant.");
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Modify assistant
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("ModifyAssistant")]
        public async Task<IActionResult> ModifyAssistant(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "assistants/{assistantId}")]
            HttpRequest req,
            string assistantId,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<AssistantRequest>(requestBody);

            if (string.IsNullOrEmpty(request.Instructions))
            {
                return new BadRequestObjectResult("Instructions is required.");
            }

            try
            {
                return new OkObjectResult(await _openAIService.ModifyAssistantAsync(request, assistantId));
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error modifying the assistant.");
                return new StatusCodeResult(500);
            }
        }
    }
}