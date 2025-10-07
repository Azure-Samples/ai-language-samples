// Copyright (c) Microsoft Corporation.
//  Licensed under the MIT License.

using System.ComponentModel;
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
    /// Provides functionality to generate a summary for a given text input.
    /// </summary>
    /// <remarks>The <see cref="SummarizationTool"/> class allows users to summarize text by specifying
    /// various parameters such as model version, language, summarization type, summary length, and sentence count. The
    /// results are returned as a JSON array of documents containing the summary. This class is designed to be used with
    /// an <see cref="ILanguageClient"/> for text analysis operations.</remarks>
    [McpServerToolType]
    public sealed class SummarizationTool
    {
        private const string Description = "This tool generates a summary for any text and returns it. " +
            "It requires the message to be summarized, and optionally accepts parameters like model version, language, summarization type, summary length bucket, and sentence count." +
            "Results are returned as a JSON array of documents containing the summary.";

        private readonly ILogger<SummarizationTool> logger;
        private readonly ILanguageClient client;

        public SummarizationTool(ILogger<SummarizationTool> logger, ILanguageClient client)
        {
            ArgumentNullException.ThrowIfNull(client);

            this.logger = logger ?? new ConsoleLogger<SummarizationTool>();
            this.client = client;
        }

        [McpServerTool(Title = "Summarize text tool")]
        [Description(Description)]
        public async Task<string> SummarizeText(
            [Description("The text that needs to be used for analysis")] string message,
            [Description("(Optional) Model version to be used.")] string modelVersion = "latest",
            [Description("(Optional) The language of the input text.  The language requested should be according to the 2 letter ISO 639-1 standard.")] string language = "en",
            [Description("(Optional) The type of summarization - extractive of abstractive. Default is abstractive.")] string summarizationType = "abstractive",
            [Description("(Optional) The length of the summary - short, medium, or long. Default is medium.")] string summaryLengthBucket = "medium",
            [Description("(Optional) The number of sentences to include in the summary. Default is null (no limit).")] int? sentenceCount = default)
        {
            try
            {
                var input = AnalyzeTextClientHelper.CreateAnalyzeTextSummarizationOperationsInput(message, language, modelVersion, summarizationType, summaryLengthBucket, sentenceCount);
                AnalyzeTextOperationState response = await this.client.AnalyzeTextAsync(input);

                var serializedResponse = response.Serialize();
                return new McpResponse(serializedResponse).Serialize();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in text summarization: {Message}", ex.Message);
                return new McpResponse(ex.Message, isError: true).Serialize();
            }
        }
    }
}
