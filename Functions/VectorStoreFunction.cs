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
    public class VectorStoresFunction
    {
        private readonly IOpenAIService _openAIService;

        public VectorStoresFunction(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        [FunctionName("SearchVectorStore")]
        public async Task<IActionResult> Search(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "vector-store/search")] HttpRequestMessage req,
            ILogger log)
        {
            string requestBody = await req.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string query = data?.query;
            if (string.IsNullOrEmpty(query))
            {
                return new BadRequestObjectResult("Query is required.");
            }

            try
            {
                string response = await _openAIService.SearchVectorStoreAsync(query);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error searching vector store.");
                return new StatusCodeResult(500);
            }
        }

        [FunctionName("InsertVector")]
        public async Task<IActionResult> Insert(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "vector-store/insert")] HttpRequestMessage req,
            ILogger log)
        {
            string requestBody = await req.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string vectorData = data?.vectorData;
            if (string.IsNullOrEmpty(vectorData))
            {
                return new BadRequestObjectResult("Vector data is required.");
            }

            try
            {
                string response = await _openAIService.InsertVectorAsync(vectorData);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error inserting vector data.");
                return new StatusCodeResult(500);
            }
        }
    }
}