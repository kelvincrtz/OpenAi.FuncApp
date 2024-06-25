using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
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

        /// <summary>
        /// Create vector store
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("CreateVectorStore")]
        public async Task<IActionResult> CreateVectorStore(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "vector_stores")]
            HttpRequest req,
            ILogger log)
        {
            try
            {
                dynamic response = await _openAIService.CreateVectorStore();
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error creating vector store.");
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// List vector stores
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("ListVectorStores")]
        public async Task<IActionResult> ListVectorStores(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "vector_stores/{vectorStoreId}")]
            HttpRequest req,
            string vectorStoreId,
            ILogger log)
        {
            if (string.IsNullOrEmpty(vectorStoreId))
            {
                return new BadRequestObjectResult("Vectore store id is required.");
            }

            try
            {
                dynamic response = await _openAIService.ListVectorStoreFiles(vectorStoreId);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error listing vector stores.");
                return new StatusCodeResult(500);
            }
        }
    }
}