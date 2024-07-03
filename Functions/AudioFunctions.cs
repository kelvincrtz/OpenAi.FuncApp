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
    [ApiExplorerSettings(GroupName = "Audio")]
    public class AudioFunctions
    {
        private readonly IOpenAIService _openAIService;

        /// <summary>
        /// </summary>
        /// <param name="openAIService"></param>
        public AudioFunctions(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        /// <summary>
        /// Create vector store
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("CreateSpeech")]
        public async Task<IActionResult> CreateSpeech(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "audio")]
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<SpeechRequest>(requestBody);

            if (string.IsNullOrEmpty(request.Input))
            {
                return new BadRequestObjectResult("Input is required for TTS.");
            }

            try
            {
                var audioData = await _openAIService.CreateSpeechAsync(request);
                return new FileContentResult(audioData, $"audio/{request.ResponseFormat}")
                {
                    FileDownloadName = $"tts_output.{request.ResponseFormat}"
                };
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error starting a new thread.");
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// List vector stores
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("CreateTranscription")]
        public async Task<IActionResult> CreateTranscription(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "audio/transcription")]
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
                var response = await _openAIService.CreateTranscriptionAsync(audioData, file.FileName, model, language, prompt, responseFormat);
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