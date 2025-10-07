// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Azure.AI.Language.MCP.Common.Response;
using Azure.AI.Language.MCP.Server.Clients.Language;
using Azure.AI.Language.MCP.Server.Clients.Language.Text;
using Azure.AI.Language.MCP.Server.Clients.Utils;
using Azure.AI.Language.MCP.Server.Enums.Ner;
using Azure.AI.Language.MCP.Server.Utils;
using Azure.AI.Language.Text;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

[assembly: InternalsVisibleTo("Azure.AI.Language.MCP.Server.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Azure.AI.Language.MCP.Server.Language.Tools
{
    /// <summary>
    /// This class provides methods for detecting named entities from text using Azure.AI.Language APIs.
    /// </summary>
    [McpServerToolType]
    public sealed class ExtractEntitiesTool
    {
        private const string NERDescription = "This tool extracts entities from the text input and returns an array of entities." +
            "It requires the message but can accept optional parameters like " +
            "- entity types to be includes, " +
            "- entity types to be excluded, " +
            "- entity overlap policy to be used. " +
            "Results are returned as a JSON array of entities. Return the response as is and don't add additional entities.";

        private readonly ILogger<ExtractEntitiesTool> logger;
        private readonly ILanguageClient client;

        public ExtractEntitiesTool(ILogger<ExtractEntitiesTool> logger, ILanguageClient client)
        {
            ArgumentNullException.ThrowIfNull(client);

            this.logger = logger;
            this.client = client;
        }

        /// <summary>
        /// Extracts named entities from the provided text using Azure.AI.Language APIs.
        /// </summary>
        /// <param name="message">The text that needs to be used for analysis.</param>
        /// <param name="inclusionList">(Optional) The list of type of entities to be included in the response. The allowed values are string representations of the enum NerTypeEnum. Default is an empty list. Make sure you map the values to the types in the enum as incorrect mappings will cause the system to throw an error.</param>
        /// <param name="exclusionList">(Optional) The list of entity types to be excluded. The types allowed are string representations of the enum NerTypeEnum. Default is empty list.</param>
        /// <param name="overlapPolicy">(Optional) Entity overlap policy allowed values: AllowOverlap, MatchLongest(default).</param>
        /// <param name="language">(Optional) The language of the input text. The language requested should be according to the 2 letter ISO 639-1 standard.</param>
        /// <param name="modelVersion">(Optional) Model version to be used.</param>
        /// <returns>A JSON string containing the array of extracted entities or an error message.</returns>
        [McpServerTool(Title = "Named Entity Extraction tool for text")]
        [Description(NERDescription)]
        public async Task<string> ExtractNamedEntitiesFromText(
            [Description("The text that needs to be used for analysis")] string message,
            [Description("(Optional) The list of type of entities to be included in the response. The allowed values are string representations of the enum NerTypeEnum. Default is an empty list. Make sure you map the values to the types in the enum as incorrect mappings will cause the system to throw an error.")] IList<string>? inclusionList = default,
            [Description("(Optional) The list of entity types to be excluded. The types allowed are string representations of the enum NerTypeEnum. Default is empty list.")] IList<string>? exclusionList = default,
            [Description("(Optional) Entity overlap policy allowed values: AllowOverlap, MatchLongest(default)")] string? overlapPolicy = default,
            [Description("(Optional) The language of the input text. The language requested should be according to the 2 letter ISO 639-1 standard.")] string language = "en",
            [Description("(Optional) Model version to be used.")] string modelVersion = "latest")
        {
            try
            {
                var inclusionListEnums = inclusionList?.ToEnumValues<NerTypeEnum>() ?? [];
                var exclusionListEnums = exclusionList?.ToEnumValues<NerTypeEnum>() ?? [];
                var overlapPolicyEnum = overlapPolicy?.ToEnumValue<OverlapPolicyEnum>() ?? OverlapPolicyEnum.MatchLongest;

                var input = AnalyzeTextClientHelper.CreateTextEntityRecognitionInput(message, language, modelVersion, inclusionListEnums, exclusionListEnums, overlapPolicyEnum);
                AnalyzeTextResult response = await this.client.AnalyzeTextAsync(input);

                var serializedResponse = response.Serialize();
                return new McpResponse(serializedResponse).Serialize();
            }
            catch (Exception ex)
            {
                this.logger.LogInformation(ex, $"Error in Ner: {ex.Message}");
                return new McpResponse(ex.Message, isError: true).Serialize();
            }
        }
    }
}