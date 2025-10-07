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
    /// This class provides methods for extracting key phrases from text using Azure.AI.Language APIs.
    /// </summary>
    [McpServerToolType]
    public sealed class ExtractKeyPhraseTool
    {
        private const string Description = "This tool extracts key phrases from the input text." +
            "It also accepts an optional model version and language. The result is a json response containing the extracted key phrases.";

        private readonly ILogger<ExtractKeyPhraseTool> logger;
        private readonly ILanguageClient client;

        public ExtractKeyPhraseTool(ILogger<ExtractKeyPhraseTool> logger, ILanguageClient client)
        {
            ArgumentNullException.ThrowIfNull(client);

            this.logger = logger ?? new ConsoleLogger<ExtractKeyPhraseTool>();
            this.client = client;
        }

        [McpServerTool(Title = "Extract key phrases from text tool")]
        [Description(Description)]
        public async Task<string> ExtractKeyPhrasesFromText(
            [Description("The text that needs to be used for analysis")] string message,
            [Description("(Optional) Model version to be used.")] string modelVersion = "latest",
            [Description("(Optional) The language of the input text.  The language requested should be according to the 2 letter ISO 639-1 standard.")] string language = "en")
        {
            try
            {
                var input = AnalyzeTextClientHelper.CreateExtractKeyPhraseInput(message, language, modelVersion);
                AnalyzeTextResult response = await this.client.AnalyzeTextAsync(input);

                var serializedResponse = response.Serialize();
                return new McpResponse(serializedResponse).Serialize();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in Extract Key Phrase analysis: {Message}", ex.Message);
                return new McpResponse("An error occurred while extracting key phrases. Please try again later.", isError: true).Serialize();
            }
        }
    }
}
