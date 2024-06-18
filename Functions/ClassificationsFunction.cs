using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenAi.FuncApp.Services.Interface;

namespace OpenAi.FuncApp.Services
{
    public class ClassificationsFunction
    {
        private readonly IOpenAIService _openAIService;

        public ClassificationsFunction(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        [FunctionName("ClassifyText")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "classifications")] HttpRequestMessage req,
            ILogger log)
        {
            string requestBody = await req.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string text = data?.text;
            if (string.IsNullOrEmpty(text))
            {
                return new BadRequestObjectResult("Text is required.");
            }

            try
            {
                string response = await _openAIService.ClassifyTextAsync(text);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error classifying text.");
                return new StatusCodeResult(500);
            }
        }
    }
}