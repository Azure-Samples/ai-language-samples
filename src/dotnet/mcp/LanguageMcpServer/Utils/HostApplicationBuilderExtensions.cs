// Copyright (c) Microsoft Corporation.
//  Licensed under the MIT License.

using System.Diagnostics;
using System.Reflection;
using Azure.AI.Language.Conversations;
using Azure.AI.Language.MCP.Common.Logger;
using Azure.AI.Language.MCP.Common.Settings;
using Azure.AI.Language.MCP.Server.Clients.Language;
using Azure.AI.Language.MCP.Server.Language.Tools;
using Azure.AI.Language.QuestionAnswering;
using Azure.AI.Language.Text;
using LanguageAgentTools.Clients.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Azure.AI.Language.MCP.Server.Utils
{
    public static class HostApplicationBuilderExtensions
    {
        public static void ConfigureLogging(this HostApplicationBuilder builder)
        {
            builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

            var fileSection = builder.Configuration.GetSection("Logging:File");
            if (fileSection.Exists())
            {
                var logfile = fileSection.GetValue<string>("Path") ?? "logs/app.log";
                long maxSize = fileSection.GetValue<long>("FileSizeLimitBytes");
                int maxFileCount = fileSection.GetValue<int>("MaxRollingFiles");
                builder.Logging.AddSimpleFileLogger(logfile, maxSize, maxFileCount);
            }

            // Optional: Add other providers
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
        }

        /// <summary>
        /// Loads config settings from JSON files and environment variables.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        public static AzureAIResourceSettings ConfigureAppSettings(this HostApplicationBuilder builder)
        {
            string exePath = Process.GetCurrentProcess().MainModule?.FileName ?? Assembly.GetExecutingAssembly().Location;
            string exeDirectory = Path.GetDirectoryName(exePath) ?? throw new ArgumentNullException("The directory containing the executable could not be determined.");

            builder.Configuration
                .AddJsonFile(Path.Combine(exeDirectory, "appsettings.json"), optional: false, reloadOnChange: true)
                .AddJsonFile(Path.Combine(exeDirectory, "appsettings.development.json"), optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            builder.Services.Configure<AzureAIResourceSettings>(builder.Configuration.GetSection("LanguageSettings"));

            using var serviceProvider = builder.Services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<IOptions<AzureAIResourceSettings>>().Value;
        }

        public static void ConfigureClientsForTools(this HostApplicationBuilder builder, ISet<string> tools)
        {
            AzureAIResourceSettings appSettings = builder.ConfigureAppSettings();

            builder.InitializeLanguageTextClient(appSettings);

            if (tools.Contains(nameof(ConversationalUnderstandingTool)))
            {
                builder.InitializeLanguageConversationsClient(appSettings);
            }

            if (tools.Contains(nameof(QuestionAnsweringTool)))
            {
                builder.InitializeQuestionAnsweringClient(appSettings);
            }
        }

        public static void InitializeLanguageTextClient(this HostApplicationBuilder builder, AzureAIResourceSettings settings)
        {
            (var endpointUri, var apiKey, _) = ExtractAzureAISettings(settings);

            builder.Services.AddSingleton<ILanguageClient>(p =>
            {
                string apiVersion = "2024-11-15-preview";

                var credential = new AzureKeyCredential(apiKey);
                var options = new TextAnalysisClientOptions(AnalyzeDocumentsClientHelper.GetTextAnalyticsServiceVersion(apiVersion));
                var client = new LanguageClient(endpointUri, credential, options);
                return client;
            });
        }

        public static void InitializeLanguageConversationsClient(this HostApplicationBuilder builder, AzureAIResourceSettings settings)
        {
            (var endpointUri, var apiKey, _) = ExtractAzureAISettings(settings);

            builder.Services.AddSingleton(new ConversationAnalysisClient(endpointUri, new AzureKeyCredential(apiKey)));
        }

        public static void InitializeQuestionAnsweringClient(this HostApplicationBuilder builder, AzureAIResourceSettings settings)
        {
            (var endpointUri, var apiKey, _) = ExtractAzureAISettings(settings);

            builder.Services.AddSingleton(p => new QuestionAnsweringClient(endpointUri, new AzureKeyCredential(apiKey)));
        }

        private static (Uri endpointUri, string apiKey, string region) ExtractAzureAISettings(AzureAIResourceSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings, "Azure AI Resource section are not configured. Please update the credentials in the appSettings.");

            var endpoint = settings.Endpoint;
            var apiKey = settings.ApiKey;
            var region = settings.Region;
            if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var endpointUri))
            {
                throw new ArgumentException($"Invalid endpoint. {endpoint}. Please update the credentials in the appSettings.");
            }

            return (endpointUri, apiKey, region);
        }
    }
}
