// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.AI.Language.MCP.Server.Clients.Language;
using Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models;
using Azure.AI.Language.MCP.Server.Contracts;
using Azure.AI.Language.MCP.Server.Response;
using Azure.AI.Language.Text;
using LanguageAgentTools.Clients.Utils;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Azure.AI.Language.MCP.Server.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Azure.AI.Language.MCP.Server.Tools
{
    /// <summary>
    /// This class provides methods for detecting and redacting Personally Identifiable Information (PII) from text and documents using Azure.AI.Language APIs.
    /// </summary>
    [McpServerToolType]
    internal sealed class PiiRedactionTool
    {
        private const string TextPIIDescription = "Detects PII from the text input and redacts it to remove PII and returns it. " +
            "It requires the message but can accept optional parameters like entity types to be redacted, entity types to be excluded, redaction policy to be used. " +
            "Results are returned as a JSON array of documents";


        private const string DocumentPIIDescription = "Detects PII from word and PDF documents in Azure Storage blobs and redacts it to remove PII. It requires the source and destination urls but can accept optional parameters like entity types to be redacted, entity types to be excluded, redaction policy to be used. " +
            "Results are returned as a JSON array of documents containing the location of the redacted documents.";
        
        
        private readonly ILogger logger;
        private readonly ILanguageClient client;

        public PiiRedactionTool(ILogger<PiiRedactionTool> logger, ILanguageClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            this.logger = logger;
            this.client = client;
        }
        [McpServerTool(Title = "Text PII redaction tool" ), Description(TextPIIDescription)]
        public async Task<string> RedactPiiFromText(
            [Description("The text that needs to be redated")] string message,
            [Description("(Optional) The list of Pii Categories to be redacted. The PII Categories allowed are string representations of the enum PiiCategoryEnum. Default is an empty list")] IList<string> piiCategories = default,
            [Description("(Optional) The list of Pii categories to be excluded from redaction. The PII Categories allowed are string representations of the enum PiiCategoryEnum. Default is empty list.")] IList<string> excludePiiCategories = default,
            [Description("(Optional) PII redaction policy")] RedactionPolicyEnum redactionPolicyEnum = RedactionPolicyEnum.CharacterMask,
            [Description("(Optional) Redaction character to be used to mask the PII. Default is *. Other supported characters are $,!,#,%,&,+,-,=,?,@,^,-,~")] char redactionCharacter = '*',
            [Description("(Optional) The language of the input text. The language requested should be according to the 2 letter ISO 639-1 standard.")] string language = "en",
            [Description("(Optional) Model version to be used.")] string modelVersion = "latest")
        {
            try
            {
                var input = LanguageClientHelper.CreateTextAnalysisTaskInput(message, piiCategories, excludePiiCategories, redactionPolicyEnum, redactionCharacter, language, modelVersion);
                AnalyzeTextResult response = await client.AnalyzeAsync(input);

                var serializedResponse = response.ToPiiEntityRecognitionResult();
                return new McpResponse(serializedResponse).Serialize();

            }
            catch (Exception ex)
            {
                logger.LogInformation(ex, $"Error in PiiRedact: {ex.Message}");
                return new McpResponse(ex.Message, isError: true).Serialize();
            }
        }

        [McpServerTool(Title = "Document PII redaction tool"), Description(DocumentPIIDescription)]
        public async Task<string> RedactPiiFromDocument(
            [Description("The Uri of the blob to be redacted.")] Uri sourceDocument,
            [Description("The Uri of the container where the redacted document and results will be stored")] Uri targetDocument,
            [Description("(Optional) The list of PII Categories to be redacted. The PII Categories allowed are string representations of the enum PiiCategoryEnum. Default is an empty list")] IList<string> piiCategories = default,
            [Description("(Optional) The list of Pii categories to be excluded from redaction. The PII Categories allowed are string representations of the enum PiiCategoryEnum. Default is empty list.")] IList<string> excludePiiCategories = default,
            [Description("(Optional) PII redaction policy. Allowed values are EntityMask, CharacterMask and NoMask")] string redactionPolicy = "CharacterMask",
            [Description("(Optional) Redaction character to be used to mask the PII. Default is *. Other supported characters are $,!,#,%,&,+,-,=,?,@,^,-,~")] char redactionCharacter = '*',
            [Description("(Optional) The language of the input text.  The language requested should be according to the 2 letter ISO 639-1 standard.")] string language = "en",
            [Description("(Optional) Model version to be used.")] string modelVersion = "latest")
        {
            try
            {            
                DocumentPiiEntityRecognitionInput input = LanguageClientHelper.CreateDocumentAnalysisTaskInput(sourceDocument, targetDocument, piiCategories, excludePiiCategories, redactionPolicy, redactionCharacter, language, modelVersion);
                var response = await client.AnalyzeDocumentAsync(input);
                var serializedResponse = response.ToDocumentPiiEntityRecognitionResult();

                return new McpResponse(serializedResponse).Serialize();
            }
            catch (Exception ex)
            {
                logger.LogInformation(ex, $"Error in DocumentPiiRedact: {ex.Message}");
                return new McpResponse(ex.Message, isError: true).Serialize();
            }
        }
    }
}