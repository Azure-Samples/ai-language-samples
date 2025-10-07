// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models;
using Azure.AI.Language.MCP.Server.Enums.Pii;
using Azure.AI.Language.Text;
using static Azure.AI.Language.Text.TextAnalysisClientOptions;

namespace LanguageAgentTools.Clients.Utils
{
    /// <summary>
    /// Provides helper methods for creating text analysis task inputs and transforming results.
    /// </summary>
    internal static class AnalyzeDocumentsClientHelper
    {
        private const string DefaultInputId = "1";

        /// <summary>
        /// Creates an input for document analysis tasks, including PII entity recognition and redaction.
        /// </summary>
        /// <param name="source">The URI of the source document to analyze.</param>
        /// <param name="target">The URI of the target document for output.</param>
        /// <param name="language">The language code of the input document.</param>
        /// <param name="modelVersion">The version of the model to use for analysis.</param>
        /// <param name="piiCategories">A list of PII categories to include in the analysis.</param>
        /// <param name="excludePiiCategories">A list of PII categories to exclude from the analysis.</param>
        /// <param name="redactionPolicy">The redaction policy to apply to detected PII entities.</param>
        /// <param name="redactionCharacter">The character to use for character masking redaction.</param>
        /// <returns>A <see cref="DocumentPiiEntityRecognitionInput"/> configured for the specified parameters.</returns>
        public static DocumentPiiEntityRecognitionInput CreateDocumentAnalysisTaskInput(
            Uri source,
            Uri target,
            string language,
            string modelVersion,
            IList<PiiCategoryEnum> piiCategories,
            IList<PiiCategoryEnum> excludePiiCategories,
            RedactionPolicyEnum redactionPolicy,
            char redactionCharacter)
        {
            AnalyzeDocumentsInput input = CreateDocumentAnalysisPayload(source, target, language);

            var tasks = new AnalyzeDocumentsTask[]
            {
                new AnalyzeDocumentsTask()
                {
                    Kind = AnalyzeDocumentsTaskKind.PiiEntityRecognition,
                    Parameters = new DocumentPiiEntityRecognitionTaskParameters(piiCategories, excludePiiCategories, new RedactionPolicy(redactionPolicy.ToString(), redactionCharacter), modelVersion),
                },
            };

            return new DocumentPiiEntityRecognitionInput(input, tasks);
        }

        /// <summary>
        /// Converts a <see cref="RedactionPolicyEnum"/> value to its corresponding <see cref="RedactionPolicyKind"/> value.
        /// </summary>
        /// <param name="redactionPolicyEnum">The redaction policy enumeration value.</param>
        /// <returns>The corresponding <see cref="RedactionPolicyKind"/>.</returns>
        public static RedactionPolicyKind ToRedactionPolicyKind(this RedactionPolicyEnum redactionPolicyEnum)
        {
            return redactionPolicyEnum switch
            {
                RedactionPolicyEnum.EntityMask => RedactionPolicyKind.EntityMask,
                RedactionPolicyEnum.NoMask => RedactionPolicyKind.NoMask,
                _ => RedactionPolicyKind.CharacterMask,
            };
        }

        /// <summary>
        /// Gets the <see cref="ServiceVersion"/> for text analytics based on the provided API version string.
        /// </summary>
        /// <param name="apiVersion">The API version string (e.g., "2023-04-01").</param>
        /// <returns>The corresponding <see cref="ServiceVersion"/>. If the version is not recognized, returns the latest available version.</returns>
        public static ServiceVersion GetTextAnalyticsServiceVersion(string apiVersion)
        {
            if (!string.IsNullOrEmpty(apiVersion))
            {
                var serviceVersion = $"Description{apiVersion.Replace('-', '_')}";
                if (Enum.TryParse<ServiceVersion>(serviceVersion, out var version))
                {
                    return version;
                }
            }

            return Enum.GetValues(typeof(ServiceVersion)).Cast<ServiceVersion>().Last();
        }

        private static AnalyzeDocumentsInput CreateDocumentAnalysisPayload(Uri source, Uri target, string language)
        {
            var input = new AnalyzeDocumentsInput();

            var sourceLocation = new DocumentLocation(source.ToString());
            var targetLocation = new DocumentLocation(target.ToString());

            input.Documents.Add(new DocumentInput(DefaultInputId, sourceLocation, targetLocation, language));
            return input;
        }
    }
}
