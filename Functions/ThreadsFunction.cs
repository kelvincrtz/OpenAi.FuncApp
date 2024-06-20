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

        [FunctionName("StartNewThread")]
        public async Task<IActionResult> StartNewThread(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "threads/start")]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<MessageRequest>(requestBody);

            try
            {
                var response = await _openAIService.StartNewThreadAsync(request);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error assisting.");
                return new StatusCodeResult(500);
            }
        }

        [FunctionName("CreateThreadAsync")]
        public async Task<IActionResult> CreateThreadAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "create/thread")]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<ThreadRequest>(requestBody);

            try
            {
                var response = await _openAIService.CreateThreadAsync(request);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error assisting.");
                return new StatusCodeResult(500);
            }
        }

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
                log.LogError(ex, "Error assisting.");
                return new StatusCodeResult(500);
            }
        }
    }
}