// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Azure.AI.Language.MCP.Server.Tools;
using Azure.AI.Language.Text;
using Azure.AI.Translation.Text;
using LanguageAgentTools.Clients.DocumentAnalysis;
using LanguageAgentTools.Clients.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Azure.AI.Language.MCP.Server
{
    /// <summary>
    /// Entry point and config for the Language MCP Server.
    /// Initializes and configures services and tools for language processing, such as PII redaction and translation.
    /// </summary>
    public class LanguageMcpServer
    {
        private const string LoggerScope = "LanguageAgentTools";

        /// <summary>
        /// Main entry point for the Language MCP Server application.
        /// Configures logging, parses tool arguments, initializes settings, and registers required services and tools.
        /// </summary>
        /// <param name="args">
        /// Command-line arguments. The first argument can specify which tool to enable: "PiiRedactionTool", "TranslatorTool", or both by default.
        /// </param>
        public static async Task Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            IMcpServerBuilder mcpServerBuilder = builder.Services
                .AddMcpServer()
                .WithStdioServerTransport();

            // Parse tools requested argument
            var tools = new List<string> { nameof(PiiRedactionTool), nameof(TranslatorTool) };
            if (args.Length > 0)
            {
                var arg = args[0]?.Trim();

                if (string.Equals(arg, nameof(PiiRedactionTool), StringComparison.InvariantCultureIgnoreCase))
                {
                    tools = new List<string> { nameof(PiiRedactionTool) };
                }
                else if (string.Equals(arg, nameof(TranslatorTool), StringComparison.InvariantCultureIgnoreCase))
                {
                    tools = new List<string> { nameof(TranslatorTool) };
                }
            }

            string exePath = Process.GetCurrentProcess().MainModule?.FileName ?? System.Reflection.Assembly.GetExecutingAssembly().Location;
            string exeDirectory = Path.GetDirectoryName(exePath);

            // initialize settings
            InitializeSettings(builder, exeDirectory);

            builder.Services.AddSingleton(p => p.GetService<ILoggerFactory>().CreateLogger(LoggerScope));

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

        /// <summary>
        /// Registers and configures the PII Redaction Tool and its dependencies.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <param name="mcpServerBuilder">The MCP server builder.</param>
        internal static void InitializePiiRedactionTool(HostApplicationBuilder builder, IMcpServerBuilder mcpServerBuilder)
        {
            var config = builder.Configuration;

            if (!Uri.TryCreate(config["LanguageSettings:Endpoint"], UriKind.Absolute, out var endpoint))
            {
                throw new ArgumentException($"Invalid LanguageSettings endpoint. {config["LanguageSettings:Endpoint"]}. Please update the credentials in the appSettings.");
            }

            builder.Services.AddSingleton(p =>
            {
                var apiKey = config["LanguageSettings:ApiKey"];
                string apiVersion = "2024-11-15-preview";

                var credential = new AzureKeyCredential(apiKey);
                var options = new TextAnalysisClientOptions(LanguageClientHelper.GetTextAnalyticsServiceVersion(apiVersion));
                var client = new DocumentAnalysisClient(endpoint, credential, options);
                return client;
            });
            mcpServerBuilder.WithTools<PiiRedactionTool>();
        }

        /// <summary>
        /// Registers and configures the Translator Tool and its dependencies.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <param name="mcpServerBuilder">The MCP server builder.</param>
        internal static void InitializeTranslatorTool(HostApplicationBuilder builder, IMcpServerBuilder mcpServerBuilder)
        {
            var config = builder.Configuration;

            if(!Uri.TryCreate(config["TranslatorSettings:Endpoint"], UriKind.Absolute, out var endpoint))
            {
                throw new ArgumentException($"Invalid Translator endpoint. {config["TranslatorSettings:Endpoint"]}. Please update the credentials in the appSettings.");
            }

            builder.Services.AddSingleton(
                p =>
                {
                    var apiKey = config["TranslatorSettings:ApiKey"];
                    var region = config["TranslatorSettings:Region"];

                    var client = new TextTranslationClient(new AzureKeyCredential(apiKey), endpoint, region: region);
                    return client;
                });

            mcpServerBuilder.WithTools<TranslatorTool>();
        }

        /// <summary>
        /// Loads config settings from JSON files and environment variables.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <param name="exeDirectory">The directory containing the executable and config files.</param>
        private static void InitializeSettings(HostApplicationBuilder builder, string exeDirectory)
        {
            builder.Configuration
                    .AddJsonFile($"{exeDirectory}/appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"{exeDirectory}/appsettings.development.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
        }
    }
}