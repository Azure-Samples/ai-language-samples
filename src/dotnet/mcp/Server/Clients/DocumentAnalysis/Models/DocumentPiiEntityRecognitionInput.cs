// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace LanguageAgentTools.Clients.DocumentAnalysis.Models
{
    /// <summary>
    /// Represents the input for document PII entity recognition.
    /// </summary>
    public class DocumentPiiEntityRecognitionInput : IAnalyzeDocumentsInput
    {
        /// <summary>
        /// Gets or sets the analysis input for the document.
        /// </summary>
        public AnalyzeDocumentsInput AnalysisInput { get; set; }

        /// <summary>
        /// Gets or sets the action content for PII entity recognition.
        /// </summary>
        public IList<AnalyzeDocumentsTask> Tasks { get; set; }

        /// <summary>
        /// Gets or sets the kind of analysis to be performed.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public AnalyzeDocumentsTaskKind Kind { get; } = AnalyzeDocumentsTaskKind.PiiEntityRecognition;

        public DocumentPiiEntityRecognitionInput(AnalyzeDocumentsInput documents, IList<AnalyzeDocumentsTask> tasks)
        {
            Tasks = tasks;
            AnalysisInput = documents;
        }
    }
}
