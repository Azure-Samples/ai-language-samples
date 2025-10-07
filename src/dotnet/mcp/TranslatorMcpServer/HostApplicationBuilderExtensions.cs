// Copyright (c) Microsoft Corporation.
//  Licensed under the MIT License.

using System.Diagnostics;
using System.Reflection;
using Azure.AI.Language.MCP.Common.Logger;
using Azure.AI.Language.MCP.Common.Settings;
using Azure.AI.Language.MCP.Server.Translator.Clients.Translator;
using Azure.AI.Language.MCP.Server.Translator.Clients.Translator.Impl;
using Azure.AI.Translation.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Azure.AI.Language.MCP.Server.Translator
{
    public static class HostApplicationBuilderExtensions
    {
        public static void ConfigureLogging(this HostApplicationBuilder builder)
        {
            builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

            var fileSection = builder.Configuration.GetSection("Logging:File");
            if (fileSection.Exists())
            {
                var logfile = fileSection.GetValue<string>("Path");
                long maxSize = fileSection.GetValue<long>("FileSizeLimitBytes");
                int maxFileCount = fileSection.GetValue<int>("MaxRollingFiles");
                builder.Logging.AddSimpleFileLogger(logfile, maxSize, maxFileCount);
            }

            // Optional: Add other providers
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
        }

        public static void ConfigureClientsForTools(this HostApplicationBuilder builder, ISet<string> tools)
        {
            AzureAIResourceSettings appSettings = builder.ConfigureAppSettings();

            builder.InitializeTranslatorClient(appSettings);
        }

        /// <summary>
        /// Loads config settings from JSON files and environment variables.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        public static AzureAIResourceSettings ConfigureAppSettings(this HostApplicationBuilder builder)
        {
            string exePath = Process.GetCurrentProcess().MainModule?.FileName ?? Assembly.GetExecutingAssembly().Location;
            string exeDirectory = Path.GetDirectoryName(exePath);

            builder.Configuration
                .AddJsonFile(Path.Combine(exeDirectory, "appsettings.json"), optional: false, reloadOnChange: true)
                .AddJsonFile(Path.Combine(exeDirectory, "appsettings.development.json"), optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            builder.Services.Configure<AzureAIResourceSettings>(builder.Configuration.GetSection("TranslatorSettings"));

            using var serviceProvider = builder.Services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<IOptions<AzureAIResourceSettings>>().Value;
        }

        public static void InitializeTranslatorClient(this HostApplicationBuilder builder, AzureAIResourceSettings settings)
        {
            (var endpointUri, var apiKey, var region) = ExtractAzureAISettings(settings);

            builder.Services.AddSingleton(p => new TextTranslationClient(new AzureKeyCredential(apiKey), endpointUri, region: region));
            builder.Services.AddSingleton<ITranslatorClient, TranslatorClient>();
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
