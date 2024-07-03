using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using OpenAi.FuncApp.Services.Interface;

namespace OpenAi.FuncApp.Functions
{
    [ApiExplorerSettings(GroupName = "Vector Store Files")]
    public class VectorStoreFilesFunction
    {
        private readonly IOpenAIService _openAIService;

        /// <summary>
        /// </summary>
        /// <param name="openAIService"></param>
        public VectorStoreFilesFunction(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        /// <summary>
        /// Create vector store file
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("CreateVectorStoreFile")]
        public async Task<IActionResult> CreateVectorStoreFile(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "vector_stores/{vectorStoreId}/files")]
            HttpRequest req,
            string vectorStoreId,
            ILogger log)
        {
            log.LogInformation("File upload request received.");

            if (!req.HasFormContentType)
            {
                return new BadRequestObjectResult("Invalid content type. Please send a form with a file.");
            }

            var form = await req.ReadFormAsync();
            var file = form.Files["file"];
            if (file == null || file.Length == 0)
            {
                return new BadRequestObjectResult("No file uploaded or file is empty.");
            }

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            var fileData = stream.ToArray();

            try
            {
                var result = await _openAIService.CreateVectorStoreFile(vectorStoreId, fileData, file.FileName);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error creating a vector store file.");
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// List vector store files
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("ListVectorStoreFiles")]
        public async Task<IActionResult> ListVectorStoreFiles(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "vector_stores/{vectorStoreId}/files")]
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
                var response = await _openAIService.ListVectorStoreFiles(vectorStoreId);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error listing vector stores.");
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Retrieve vector store file
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("RetrieveVectorStoreFile")]
        public async Task<IActionResult> RetrieveVectorStoreFile(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "vector_stores/{vectorStoreId}/files/{fileId}")]
            HttpRequest req,
            string vectorStoreId,
            string fileId,
            ILogger log)
        {
            if (string.IsNullOrEmpty(vectorStoreId) || string.IsNullOrEmpty(fileId))
            {
                return new BadRequestObjectResult("Vectore store id and file id are required.");
            }

            try
            {
                var response = await _openAIService.RetrieveVectorStoreFile(vectorStoreId, fileId);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error listing vector stores.");
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Delete vector store file
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("DeleteVectorStoreFile")]
        public async Task<IActionResult> DeleteVectorStoreFile(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "vector_stores/{vectorStoreId}/files/{fileId}")]
            HttpRequest req,
            string vectorStoreId,
            string fileId,
            ILogger log)
        {
            if (string.IsNullOrEmpty(vectorStoreId) || string.IsNullOrEmpty(fileId))
            {
                return new BadRequestObjectResult("Vectore store id and file id are required.");
            }

            try
            {
                var response = await _openAIService.DeleteVectorStoreFile(vectorStoreId, fileId);
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