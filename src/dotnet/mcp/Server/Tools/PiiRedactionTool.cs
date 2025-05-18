// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.AI.Language.MCP.Server.Contracts;
using Azure.AI.Language.Text;
using LanguageAgentTools.Clients.DocumentAnalysis;
using LanguageAgentTools.Clients.DocumentAnalysis.Models;
using LanguageAgentTools.Clients.Utils;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Azure.AI.Language.MCP.Server.Tools
{
    /// <summary>
    /// This class provides methods for detecting and redacting Personally Identifiable Information (PII) from text and documents using Azure.AI.Language APIs.
    /// </summary>
    [McpServerToolType]
    internal class PiiRedactionTool
    {
        [McpServerTool(Title = "Text PII redaction tool" ), Description("Detects PII from the text input and redacts it to remove PII. It can accept optional parameters like entity types to be redacted, entity types to be excluded, redaction policy to be used.")]
        public static string RedactPiiFromText(
            DocumentAnalysisClient client,
            [Description("The text that needs to be redated")] string message,
            [Description("(Optional) The list of Pii Categories to be redacted. The PII Categories allowed are string representations of the enum PiiCategoryEnum. Default is an empty list")] IList<string> piiCategories = default,
            [Description("(Optional) The list of Pii categories to be excluded from redaction. The PII Categories allowed are string representations of the enum PiiCategoryEnum. Default is empty list.")] IList<string> excludePiiCategories = default,
            [Description("(Optional) PII redaction policy")] RedactionPolicyEnum redactionPolicyEnum = RedactionPolicyEnum.CharacterMask,
            [Description("(Optional) Redaction character to be used to mask the PII. Default is *. Other supported characters are $,!,#,%,&,+,-,=,?,@,^,-,~")] char redactionCharacter = '*',
            [Description("(Optional) The language of the input text. The language requested should be according to the 2 letter ISO 639-1 standard.")] string language = "en",
            [Description("(Optional) Model version to be used.")] string modelVersion = "latest",
            ILogger logger = default)
        {
            try
            {
                var input = LanguageClientHelper.CreateTextAnalysisTaskInput(message, piiCategories, excludePiiCategories, redactionPolicyEnum, redactionCharacter, language, modelVersion);
                AnalyzeTextResult response = client.AnalyzeText(input);
                return response.ToPiiEntityRecognitionResult();
            }
            catch (Exception ex)
            {
                logger.LogInformation(ex, $"Error in PiiRedact: {ex.Message}");
                return $"Error in PiiRedact: {ex.Message}";
            }
        }

        [McpServerTool(Title = "Document PII redaction tool"), Description("Detects and removed PII from word and PDF documents in Azure Storage blobs. This is  a long running operation with progress updates")]
        public static async Task<string> RedactPiiFromDocument(
            DocumentAnalysisClient client,
            [Description("The Uri of the blob to be redacted.")] Uri sourceDocument,
            [Description("The Uri of the container where the redacted document and results will be stored")] Uri targetDocument,
            [Description("(Optional) The list of PII Categories to be redacted. The PII Categories allowed are string representations of the enum PiiCategoryEnum. Default is an empty list")] IList<string> piiCategories = default,
            [Description("(Optional) The list of Pii categories to be excluded from redaction. The PII Categories allowed are string representations of the enum PiiCategoryEnum. Default is empty list.")] IList<string> excludePiiCategories = default,
            [Description("(Optional) PII redaction policy. Allowed values are EntityMask, CharacterMask and NoMask")] string redactionPolicy = "CharacterMask",
            [Description("(Optional) Redaction character to be used to mask the PII. Default is *. Other supported characters are $,!,#,%,&,+,-,=,?,@,^,-,~")] char redactionCharacter = '*',
            [Description("(Optional) The language of the input text.  The language requested should be according to the 2 letter ISO 639-1 standard.")] string language = "en",
            [Description("(Optional) Model version to be used.")] string modelVersion = "latest",
            ILogger logger = default)
        {
            try
            {            
                DocumentPiiEntityRecognitionInput input = LanguageClientHelper.CreateDocumentAnalysisTaskInput(sourceDocument, targetDocument, piiCategories, excludePiiCategories, redactionPolicy, redactionCharacter, language, modelVersion);
                var response = await client.AnalyzeDocumentOperationAsync(input);
                return response.ToDocumentPiiEntityRecognitionResult();
            }
            catch (Exception ex)
            {
                logger.LogInformation(ex, $"Error in DocumentPiiRedact: {ex.Message}");
                return $"Error in DocumentPiiRedact: {ex.Message}";
            }
        }
    }
}