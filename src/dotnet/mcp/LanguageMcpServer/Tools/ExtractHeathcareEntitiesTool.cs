// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Azure.AI.Language.MCP.Common.Response;
using Azure.AI.Language.MCP.Server.Clients.Language;
using Azure.AI.Language.MCP.Server.Clients.Language.Text;
using Azure.AI.Language.MCP.Server.Clients.Utils;
using Azure.AI.Language.MCP.Server.Enums.Healthcare;
using Azure.AI.Language.MCP.Server.Utils;
using Azure.AI.Language.Text;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

[assembly: InternalsVisibleTo("Azure.AI.Language.MCP.Server.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Azure.AI.Language.MCP.Server.Language.Tools
{
    /// <summary>
    /// This class provides methods for detecting healthcare entities from text using Azure.AI.Language APIs.
    /// </summary>
    [McpServerToolType]
    public sealed class ExtractHeathcareEntitiesTool
    {
        private const string HeathCareDescription = "This tool extracts healthcare entities from the text input and returns an array of entities." +
            "It requires the message to extract entities from. It also accepts optional parameters like document types, fhir version. " +
            "Results are returned as a JSON array of entities. Return the response as is and don't add additional entities.";

        private readonly ILogger<ExtractHeathcareEntitiesTool> logger;
        private readonly ILanguageClient client;

        public ExtractHeathcareEntitiesTool(ILogger<ExtractHeathcareEntitiesTool> logger, ILanguageClient client)
        {
            ArgumentNullException.ThrowIfNull(client);

            this.logger = logger;
            this.client = client;
        }

        /// <summary>
        /// Extracts healthcare entities from the provided text using Azure.AI.Language APIs.
        /// </summary>
        /// <param name="message">(Required) The text that needs to be used for analysis.</param>
        /// <param name="healthcareDocumentTypeStr">(Optional) The healthcare document type to be used. The allowed values are string representation of the enum HealthcareDocumentTypeEnum. Default to None if not provided.</param>
        /// <param name="language">(Optional) The language of the input text. The language requested should be according to the 2 letter ISO 639-1 standard.</param>
        /// <param name="fhirVersion">(Optional) The Fast Healthcare Interoperability Resources (FHIR) version. Default is set to 4.0.1.</param>
        /// <param name="modelVersion">(Optional) Model version to be used.</param>
        /// <returns>A JSON string containing the array of extracted healthcare entities or an error message.</returns>
        [McpServerTool(Title = "Healthcare Extraction tool for text")]
        [Description(HeathCareDescription)]
        public async Task<string> ExtractHealthcareEntitiesFromText(
           [Description("(Required) The text that needs to be used for analysis")] string message,
           [Description($"(Optional) The healthcare document type to be used. The allowed values are string representation of the enum HealthcareDocumentTypeEnum. Default to None if not provided.")] string healthcareDocumentTypeStr = "None",
           [Description("(Optional) The language of the input text. The language requested should be according to the 2 letter ISO 639-1 standard.")] string language = "en",
           [Description("(Optional) The Fast Healthcare Interoperability Resources (FHIR) version. Default is set to 4.0.1")] string fhirVersion = "4.0.1",
           [Description("(Optional) Model version to be used.")] string modelVersion = "latest")
        {
            try
            {
                HealthcareDocumentTypeEnum healthcareDocumentType = healthcareDocumentTypeStr?.ToEnumValue<HealthcareDocumentTypeEnum>() ?? HealthcareDocumentTypeEnum.None;

                var input = AnalyzeTextClientHelper.CreateHealthcareAnalyzeTextOperationsInput(message, language, modelVersion, healthcareDocumentType, fhirVersion);
                AnalyzeTextOperationState response = await this.client.AnalyzeTextAsync(input);

                var serializedResponse = response.Serialize();
                return new McpResponse(serializedResponse).Serialize();
            }
            catch (Exception ex)
            {
                this.logger.LogInformation(ex, $"Error in ExtractHealthcareEntitiesFromText: {ex.Message}");
                return new McpResponse(ex.Message, isError: true).Serialize();
            }
        }
    }
}