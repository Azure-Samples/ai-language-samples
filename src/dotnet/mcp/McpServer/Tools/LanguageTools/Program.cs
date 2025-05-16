// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure;
using Azure.AI.Language.Text;
using Azure.AI.Translation.Text;
using LanguageAgentTools;
using LanguageAgentTools.Tools.Clients.DocumentAnalysis;
using LanguageAgentTools.Tools.Clients.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class Program
{
    private const string PiiRedationToolName = "piiredactiontool";
    private const string TranslatorToolName = "translatortool";

    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Logging.AddConsole(consoleLogOptions =>
        {
            // Configure all logs to go to stderr
            consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
        });

        IMcpServerBuilder mcpServerBuilder = builder.Services
            .AddMcpServer()
            .WithStdioServerTransport();

        // Parse tools requested argument
        IList<string> tools;
        if (args.Length > 0)
        {
            var arg = args[0]?.Trim().ToLowerInvariant();
            tools = arg switch
            {
                PiiRedationToolName => new List<string> { nameof(PiiRedactionTool) },
                TranslatorToolName => new List<string> { nameof(TranslatorTool) },
                _ => new List<string> { nameof(PiiRedactionTool), nameof(TranslatorTool) }
            };
        }
        else
        {
            tools = new List<string> { nameof(PiiRedactionTool), nameof(TranslatorTool) };
        }

        string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string exeDirectory = Path.GetDirectoryName(exePath);

        // initialize settings
        InitializeSettings(builder, exeDirectory);

        // add console logger.
        builder.Logging.AddConsole(options =>
        {
            options.LogToStandardErrorThreshold = LogLevel.Trace;
        });

        builder.Services.AddSingleton<ILogger>(p => p.GetService<ILoggerFactory>().CreateLogger("LanguageAgentTools"));

        // Add TextAnalysisClient if needed
        if (tools.Contains(nameof(PiiRedactionTool)))
        {
            InitializePiiRedactionTool(builder, mcpServerBuilder);
        }


        // Add translator client if needed
        if (tools.Contains(nameof(TranslatorTool)))
        {
            InitializeTranslatorTool(builder, mcpServerBuilder);
        }

        await builder.Build().RunAsync();
    }

    internal static void InitializePiiRedactionTool(HostApplicationBuilder builder, IMcpServerBuilder mcpServerBuilder)
    {
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
        mcpServerBuilder.WithTools<PiiRedactionTool>();
    }

    internal static void InitializeTranslatorTool(HostApplicationBuilder builder, IMcpServerBuilder mcpServerBuilder)
    {
        builder.Services.AddSingleton(
            p =>
            {
                var endpoint = new Uri(builder.Configuration["TranslatorSettings:Endpoint"]);
                var apiKey = builder.Configuration["TranslatorSettings:ApiKey"];
                var region = builder.Configuration["TranslatorSettings:Region"];

                TextTranslationClient client = new TextTranslationClient(new AzureKeyCredential(apiKey), endpoint, region: region);
                return client;
            });

        mcpServerBuilder.WithTools<TranslatorTool>();
    }

    private static void InitializeSettings(HostApplicationBuilder builder, string exeDirectory)
    {
        builder.Configuration
                .AddJsonFile($"{exeDirectory}/appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"{exeDirectory}/appsettings.development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
    }
}