using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using OpenAi.FuncApp.Data.Requests;
using OpenAi.FuncApp.Services.Interface;

namespace OpenAi.FuncApp.Functions
{
    public class OpenAIFunctions
    {
        private readonly IOpenAIService _openAIService;

        public OpenAIFunctions(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        /// <summary>
        /// TEST ONLY
        /// Audio Chat
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("AudioChat")]
        public async Task<IActionResult> AudioChat(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "openai/audio/chat")]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Transcription request received.");
            if (!req.HasFormContentType)
            {
                return new BadRequestObjectResult("Invalid content type. Please send a form with an audio file.");
            }

            var form = await req.ReadFormAsync();
            var file = form.Files["file"];
            if (file == null || file.Length == 0)
            {
                return new BadRequestObjectResult("No file uploaded or file is empty.");
            }

            var model = form["model"].FirstOrDefault() ?? "whisper-1";
            var language = form["language"].FirstOrDefault() ?? "en";
            var prompt = form["prompt"].FirstOrDefault() ?? "";
            var responseFormat = form["response_format"].FirstOrDefault() ?? "json";

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            var audioData = stream.ToArray();

            try
            {
                var responseAudio = await _openAIService.CreateTranscriptionAsync(audioData, file.FileName, model, language, prompt, responseFormat);

                var messageRequest = new MessageRequest
                {
                    Role = "User",
                    Content = responseAudio.Text,
                    Assistant_Id = ""
                };
                var response = await _openAIService.StartNewThreadJsonFormatAsync(messageRequest);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error starting a new thread.");
                return new StatusCodeResult(500);
            }
        }
    }
}