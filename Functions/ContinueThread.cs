using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenAI.FuncApp.Services;
using Newtonsoft.Json;

namespace OpenAI.FuncApp.Functions
{
    public class ContinueThread
    {
        private readonly ThreadService _threadService;

        public ContinueThread(ThreadService threadService)
        {
            _threadService = threadService;
        }

        [FunctionName("ContinueThread")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string threadId = data?.threadId;
            string newMessage = data?.newMessage;

            await _threadService.ContinueThread(threadId, newMessage);

            return new OkResult();
        }
    }
}
