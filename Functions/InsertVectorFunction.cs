using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MyVectorFunctionApp.Services;
using Newtonsoft.Json;

namespace MyVectorFunctionApp.Functions
{
    public class InsertVectorFunction
    {
        private readonly VectorStoreService _vectorStoreService;

        public InsertVectorFunction(VectorStoreService vectorStoreService)
        {
            _vectorStoreService = vectorStoreService;
        }

        [FunctionName("InsertVector")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string vectorId = data?.vectorId;
            double[] vectorData = data?.vectorData.ToObject<double[]>();

            if (vectorId == null || vectorData == null)
            {
                return new BadRequestObjectResult("Invalid request data");
            }

            await _vectorStoreService.InsertVectorAsync(vectorId, vectorData);
            return new OkResult();
        }
    }
}
