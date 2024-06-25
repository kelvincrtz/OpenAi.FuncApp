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
    [ApiExplorerSettings(GroupName = "Threads")]
    public class ThreadsFunction
    {
        private readonly IOpenAIService _openAIService;

        /// <summary>
        /// We use Open AI service
        /// </summary>
        /// <param name="openAIService"></param>
        public ThreadsFunction(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        /// <summary>
        /// Create a new thread
        /// This will call the Message API followed by the Run API
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("StartNewThreadAsync")]
        public async Task<IActionResult> StartNewThreadAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "threads")]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<MessageRequest>(requestBody);

            if (string.IsNullOrEmpty(request.Content) || string.IsNullOrEmpty(request.Assistant_Id))
            {
                return new BadRequestObjectResult("Content and AssistantId are required.");
            }

            try
            {
                var response = await _openAIService.StartNewThreadAsync(request);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error starting a new thread.");
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Continue an existing thread
        /// This will call the Message API followed by the Run API
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("ContinueThreadAsync")]
        public async Task<IActionResult> ContinueThreadAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "threads/{threadId}")]
            HttpRequest req,
            string threadId,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<MessageRequest>(requestBody);

            if (string.IsNullOrEmpty(request.Content) || string.IsNullOrEmpty(request.Assistant_Id))
            {
                return new BadRequestObjectResult("Content and AssistantId are required.");
            }

            try
            {
                var response = await _openAIService.ContinueThreadAsync(request, threadId);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error continuing an exisiting thread.");
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Create a new thread
        /// This will call the Message API followed by the Run API
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("StartNewThreadJsonFormatAsync")]
        public async Task<IActionResult> StartNewThreadJsonFormatAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "threads/json/json")]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<MessageRequest>(requestBody);

            if (string.IsNullOrEmpty(request.Content) || string.IsNullOrEmpty(request.Assistant_Id))
            {
                return new BadRequestObjectResult("Content and AssistantId are required.");
            }

            try
            {
                var response = await _openAIService.StartNewThreadJsonFormatAsync(request);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error starting a new thread.");
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Endpoints is expose only for testing purposes
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("CreateRun")]
        public async Task<IActionResult> CreateRun(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "create/thread/run/{threadId}")]
            HttpRequest req,
            string threadId,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<RunRequest>(requestBody);

            try
            {
                var response = await _openAIService.CreateRun(request, threadId);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error running the thread.");
                return new StatusCodeResult(500);
            }
        }
    }
}