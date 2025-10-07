// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.AI.Language.MCP.Common.Logger;
using Azure.AI.Language.MCP.Common.Response;
using Azure.AI.Language.MCP.Server.Clients.Language;
using Azure.AI.Language.MCP.Server.Clients.Language.Text;
using Azure.AI.Language.MCP.Server.Clients.Utils;
using Azure.AI.Language.Text;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace Azure.AI.Language.MCP.Server.Language.Tools
{
    /// <summary>
    /// This class provides methods for detecting sentiment from text using Azure.AI.Language APIs.
    /// </summary>
    [McpServerToolType]
    public sealed class SentimentAnalysisTool
    {
        private const string Description = "This tool detects the sentiment from the input text." +
            "It also accepts an optional model version and country hint. The result is a json response containing the detected sentiment.";

        private readonly ILogger<SentimentAnalysisTool> logger;
        private readonly ILanguageClient client;

        public SentimentAnalysisTool(ILogger<SentimentAnalysisTool> logger, ILanguageClient client)
        {
            ArgumentNullException.ThrowIfNull(client);

            this.logger = logger ?? new ConsoleLogger<SentimentAnalysisTool>();
            this.client = client;
        }

        [McpServerTool(Title = "Detect sentiment for text tool")]
        [Description(Description)]
        public async Task<string> DetectSentimentFromText(
            [Description("The text that needs to be used for analysis")] string message,
            [Description("(Optional) Whether to include opinion mining in the analysis. Default set to true.")] bool opinionMining = true,
            [Description("(Optional) Model version to be used.")] string modelVersion = "latest",
            [Description("(Optional) The language of the input text.  The language requested should be according to the 2 letter ISO 639-1 standard.")] string language = "en")
        {
            try
            {
                var input = AnalyzeTextClientHelper.CreateSentimentAnalysisInput(message, language, modelVersion, opinionMining);
                AnalyzeTextResult response = await this.client.AnalyzeTextAsync(input);

                var serializedResponse = response.Serialize();
                return new McpResponse(serializedResponse).Serialize();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in Sentiment analysis: {Message}", ex.Message);
                return new McpResponse("An error occurred while processing the sentiment analysis request.", isError: true).Serialize();
            }
        }
    }
}
