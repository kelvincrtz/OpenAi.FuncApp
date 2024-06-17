using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using OpenAi.FuncApp.Configuration;
using OpenAi.FuncApp.Services.Interface;
using OpenAI.FuncApp.Services;

[assembly: FunctionsStartup(typeof(OpenAI.FuncApp.Startup))]

namespace OpenAI.FuncApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.Configure<OpenAIConfig>(options =>
            {
                options.ApiKey = Environment.GetEnvironmentVariable("OpenAI_ApiKey");
                options.BaseUrl = Environment.GetEnvironmentVariable("OpenAI_BaseUrl");
            });
            builder.Services.AddHttpClient<IOpenAIService, OpenAIService>();
        }
    }
}