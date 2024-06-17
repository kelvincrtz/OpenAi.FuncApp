using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyVectorFunctionApp.Services;
using OpenAI.FuncApp.Services;

[assembly: FunctionsStartup(typeof(OpenAI.FuncApp.Startup))]

namespace OpenAI.FuncApp
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            builder.ConfigurationBuilder
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<OpenAIService>();
            builder.Services.AddSingleton<ThreadService>();
            builder.Services.AddSingleton<VectorStoreService>();
        }
    }
}