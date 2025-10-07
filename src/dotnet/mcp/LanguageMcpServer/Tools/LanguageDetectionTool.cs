// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Azure.AI.Language.MCP.Common.Logger;
using Azure.AI.Language.MCP.Common.Response;
using Azure.AI.Language.MCP.Server.Clients.Language;
using Azure.AI.Language.MCP.Server.Clients.Language.Text;
using Azure.AI.Language.MCP.Server.Clients.Utils;
using Azure.AI.Language.Text;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

[assembly: InternalsVisibleTo("Azure.AI.Language.MCP.Server.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Azure.AI.Language.MCP.Server.Language.Tools
{
    /// <summary>
    /// This class provides methods for detecting language used in text using Azure.AI.Language APIs.
    /// </summary>
    [McpServerToolType]
    public sealed class LanguageDetectionTool
    {
        private const string Description = "This tool detects and returns language detected. The input is the message for which we want to detect the language." +
            "It also accepts an optional model version and country hint. The result is a json response containing the detected language.";

        private readonly ILogger<LanguageDetectionTool> logger;
        private readonly ILanguageClient client;

        public LanguageDetectionTool(ILogger<LanguageDetectionTool> logger, ILanguageClient client)
        {
            ArgumentNullException.ThrowIfNull(client);

            this.logger = logger ?? new ConsoleLogger<LanguageDetectionTool>();
            this.client = client;
        }

        [McpServerTool(Title = "Detect language for text tool")]
        [Description(Description)]
        public async Task<string> DetectLanguageFromText(
            [Description("The text that needs to be used for analysis")] string message,
            [Description("(Optional) Model version to be used.")] string modelVersion = "latest",
            [Description("(Optional) Country hint represented as ISO 3166-1 alpha-2 two-letter country code")] string countryHint = default)
        {
            try
            {
                var input = AnalyzeTextClientHelper.CreateLanguageDetectionInput(message, modelVersion, countryHint);
                AnalyzeTextResult response = await this.client.AnalyzeTextAsync(input);

                var serializedResponse = response.Serialize();
                return new McpResponse(serializedResponse).Serialize();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in Language detection: {Message}", ex.Message);
                return new McpResponse(ex.Message, isError: true).Serialize();
            }
        }
    }
}