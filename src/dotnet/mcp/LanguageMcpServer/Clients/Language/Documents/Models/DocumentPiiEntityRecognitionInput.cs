// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models
{
    /// <summary>
    /// Represents the input for document PII entity recognition.
    /// </summary>
    public class DocumentPiiEntityRecognitionInput : IAnalyzeDocumentsInput
    {
        public DocumentPiiEntityRecognitionInput(AnalyzeDocumentsInput documents, IList<AnalyzeDocumentsTask> tasks)
        {
            this.Tasks = tasks;
            this.AnalysisInput = documents;
        }

        /// <summary>
        /// Gets or sets the analysis input for the document.
        /// </summary>
        public AnalyzeDocumentsInput AnalysisInput { get; set; }

        /// <summary>
        /// Gets or sets the action content for PII entity recognition.
        /// </summary>
        public IList<AnalyzeDocumentsTask> Tasks { get; set; }

        /// <summary>
        /// Gets the kind of analysis to be performed.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public AnalyzeDocumentsTaskKind Kind { get; } = AnalyzeDocumentsTaskKind.PiiEntityRecognition;
    }
}
