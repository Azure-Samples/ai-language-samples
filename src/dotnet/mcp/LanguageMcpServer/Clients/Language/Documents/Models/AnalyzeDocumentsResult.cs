// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.AI.Language.Text;

namespace Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models
{
    public class AnalyzeDocumentsResult
    {
        public IReadOnlyList<AnalyzeDocumentsTaskError> Errors { get; set; }

        /// <summary> Gets or sets if showStats=true was specified in the request this field will contain information about the request payload. </summary>
        public RequestStatistics Statistics { get; set; }

        /// <summary> Gets or sets which model is used for analysis. </summary>
        public string ModelVersion { get; set; }

        /// <summary> Gets or sets the response by document. </summary>
        public IReadOnlyList<DocumentResult> Documents { get; set; }
    }
}
