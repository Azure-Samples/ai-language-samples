// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.AI.Language.Text;

namespace Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models
{
    public class DocumentResult
    {
        /// <summary> Gets unique, non-empty document identifier. </summary>
        public string Id { get; }

        /// <summary> Gets or sets warnings encountered while processing document. </summary>
        public IReadOnlyList<DocumentWarning> Warnings { get; set; }

        /// <summary> Gets or sets if showStats=true was specified in the request this field will contain information about the document payload. </summary>
        public DocumentStatistics Statistics { get; set; }

        /// <summary>
        /// Gets or sets location of the input document.
        /// Please note <see cref="DocumentLocation"/> is the base class. According to the scenario, a derived class of the base class might need to be assigned here, or this property needs to be casted to one of the possible derived classes.
        /// The available derived classes include <see cref="AzureBlobDocumentLocation"/>.
        /// </summary>
        public DocumentLocation Source { get; set; }

        /// <summary>
        /// Gets or sets array of document results generated after the analysis.
        /// Please note <see cref="DocumentLocation"/> is the base class. According to the scenario, a derived class of the base class might need to be assigned here, or this property needs to be casted to one of the possible derived classes.
        /// The available derived classes include <see cref="AzureBlobDocumentLocation"/>.
        /// </summary>
        public IReadOnlyList<DocumentLocation> Targets { get; set; }
    }
}