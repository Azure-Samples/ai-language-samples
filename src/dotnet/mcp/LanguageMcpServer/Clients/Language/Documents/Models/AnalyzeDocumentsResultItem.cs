// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models
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
            this.Id = id;
            this.Warnings = warnings ?? new List<InputWarning>();
            this.Source = source;
            this.Targets = targets;
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
}
