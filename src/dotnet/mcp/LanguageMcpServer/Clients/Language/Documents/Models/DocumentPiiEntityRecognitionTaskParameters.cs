// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.AI.Language.MCP.Server.Enums.Pii;

namespace Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models
{
    /// <summary>
    /// Parameters for the PII task.
    /// </summary>
    public class DocumentPiiEntityRecognitionTaskParameters : IAnalyzeDocumentsTaskParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentPiiEntityRecognitionTaskParameters"/> class.
        /// </summary>
        /// <param name="piiCategories"> Categories for Pii category.</param>
        /// <param name="excludePiiCategories">Categories for excluding from PII.</param>
        /// <param name="redactionPolicy">Redaction policy.</param>
        /// <param name="modelVersion">string modelVersion.</param>
        public DocumentPiiEntityRecognitionTaskParameters(
            IList<PiiCategoryEnum> piiCategories,
            IList<PiiCategoryEnum> excludePiiCategories,
            RedactionPolicy redactionPolicy,
            string modelVersion = "latest")
        {
            if (piiCategories?.Any() ?? false)
            {
                this.PiiCategories = piiCategories.Select(e => e.ToString()).ToList();
            }

            if (excludePiiCategories?.Any() ?? false)
            {
                this.ExcludePiiCategories = excludePiiCategories.Select(e => e.ToString()).ToList();
            }

            this.RedactionPolicy = redactionPolicy;
            this.ModelVersion = modelVersion;
        }

        /// <summary>
        /// Gets the model version to use for the task.
        /// </summary>
        public string ModelVersion { get; }

        /// <summary>
        /// Gets the list of PII categories to include in the analysis.
        /// </summary>
        public IList<string> PiiCategories { get; }

        /// <summary>
        /// Gets the list of PII categories to exclude from the analysis.
        /// </summary>
        public IList<string> ExcludePiiCategories { get; }

        /// <summary>
        /// Gets parameter to provide the redaction policy.
        /// </summary>
        public RedactionPolicy RedactionPolicy { get; }
    }
}
