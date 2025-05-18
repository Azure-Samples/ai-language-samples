// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace LanguageAgentTools.Clients.DocumentAnalysis.Models
{
    /// <summary>
    /// Represents the result of a document analysis operation.
    /// </summary>
    internal class AnalyzeDocumentsResultItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyzeDocumentsResultItem"/> class.
        /// </summary>
        /// <param name="id">The unique identifier for the analysis result item.</param>
        /// <param name="source">The source document location used for analysis.</param>
        /// <param name="targets">The list of target document locations resulting from the analysis.</param>
        /// <param name="warnings">Optional warnings generated during the analysis.</param>
        public AnalyzeDocumentsResultItem(
            string id,
            DocumentLocation source,
            IList<DocumentLocation> targets,
            IEnumerable<InputWarning> warnings = null)
        {
            Id = id;
            Warnings = warnings ?? new List<InputWarning>();
            Source = source;
            Targets = targets;
        }

        /// <summary>
        /// Gets the unique identifier for the analysis result item.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the source document location used for analysis.
        /// </summary>
        [JsonProperty(Order = 2)]
        public DocumentLocation Source { get; }

        /// <summary>
        /// Gets the list of target document locations resulting from the analysis.
        /// </summary>
        [JsonProperty(Order = 3)]
        public IList<DocumentLocation> Targets { get; }

        /// <summary>
        /// Gets the collection of warnings generated during the analysis.
        /// </summary>
        public IEnumerable<InputWarning> Warnings { get; }
    }
    /// <summary>
    /// Represents a warning generated during document analysis.
    /// </summary>
    public class InputWarning
    {
        /// <summary>
        /// Gets or sets the warning code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the warning message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets the reference associated with the warning.
        /// </summary>
        public string Ref { get; private set; }
    }
}
