// Copyright (c) Microsoft Corporation.  
// Licensed under the MIT License.  

namespace LanguageAgentTools.Tools.Clients.DocumentAnalysis.Models
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
            IList<string> piiCategories,
            IList<string> excludePiiCategories,
            RedactionPolicy redactionPolicy,
            string modelVersion = "latest")
        {
            if (piiCategories?.Any() ?? false)
            {
                PiiCategories = piiCategories;
            }

            if (excludePiiCategories?.Any() ?? false)
            {
                ExcludePiiCategories = excludePiiCategories;
            }
            RedactionPolicy = redactionPolicy;
            ModelVersion = modelVersion;
        }

        /// <summary>
        /// The model version to use for the task.
        /// </summary>
        public string ModelVersion { get; } 
        /// <summary>
        /// The list of PII categories to include in the analysis.
        /// </summary>
        public IList<string> PiiCategories { get; }
        /// <summary>
        /// The list of PII categories to exclude from the analysis.
        /// </summary>
        public IList<string> ExcludePiiCategories { get; }
        /// <summary>
        /// Parameter to provide the redaction policy.
        /// </summary>
        public RedactionPolicy RedactionPolicy { get; } 
    }
}
