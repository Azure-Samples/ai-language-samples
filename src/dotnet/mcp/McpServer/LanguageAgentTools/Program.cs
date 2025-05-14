// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Azure.AI.Language.Text;
using Azure.AI.Translation.Text;
using Language.Document;
using LanguageAgentTools.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Logging.AddConsole(consoleLogOptions =>
        {
            // Configure all logs to go to stderr
            consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
        });

        builder.Services
            .AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly();

        string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string exeDirectory = Path.GetDirectoryName(exePath);

        // initialize settings
        InitializeSettings(builder, exeDirectory);

        // add console logger.
        builder.Logging.AddConsole(options =>
        {
            options.LogToStandardErrorThreshold = LogLevel.Trace;
        });

        // Add TextAnalysisClient.
        builder.Services.AddSingleton(p =>
        {
            var endpoint = new Uri(builder.Configuration["LanguageSettings:Endpoint"]);
            var apiKey = builder.Configuration["LanguageSettings:ApiKey"];
            string apiVersion = "2024-11-15-preview";

            var credential = new AzureKeyCredential(apiKey);
            var options = new TextAnalysisClientOptions(LanguageClientHelper.GetTextAnalyticsServiceVersion(apiVersion));
            var client = new DocumentAnalysisClient(endpoint, credential, options);
            return client;
        });

        builder.Services.AddSingleton<ILogger>(p => p.GetService<ILoggerFactory>().CreateLogger("LanguageAgentTools"));

        // add translator client
        builder.Services.AddSingleton(
            p =>
            {
                var endpoint = new Uri(builder.Configuration["TranslatorSettings:Endpoint"]);
                var apiKey = builder.Configuration["TranslatorSettings:ApiKey"]; 
                var region = builder.Configuration["TranslatorSettings:Region"]; 

                TextTranslationClient client = new TextTranslationClient(new AzureKeyCredential(apiKey), endpoint, region: region);
                return client;
            });
        await builder.Build().RunAsync();
    }

    private static void InitializeSettings(HostApplicationBuilder builder, string exeDirectory)
    {
        builder.Configuration
                .AddJsonFile($"{exeDirectory}/appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{exeDirectory}/appsettings.development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
    }
}