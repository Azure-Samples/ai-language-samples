// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.AI.Language.Text;

namespace Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models
{
    public class AnalyzeDocumentsJobState
    {
        /// <summary> Gets or sets display name. </summary>
        public string DisplayName { get; set; }

        /// <summary> Gets or sets Date and time job created. </summary>
        public DateTimeOffset CreatedDateTime { get; set; }

        /// <summary> Gets or sets Date and time job expires. </summary>
        public DateTimeOffset? ExpirationDateTime { get; set; }

        /// <summary> Gets or sets job ID. </summary>
        public Guid JobId { get; set; }

        /// <summary> Gets or sets last updated date and time. </summary>
        public DateTimeOffset LastUpdatedDateTime { get; set; }

        /// <summary> Gets or sets status. </summary>
        public string Status { get; set; }

        /// <summary> Gets or sets errors. </summary>
        public IReadOnlyList<AnalyzeDocumentsError> Errors { get; set; }

        /// <summary> Gets or sets error. </summary>
        public AnalyzeDocumentsError Error { get; set; }

        /// <summary> Gets or sets next link. </summary>
        public string NextLink { get; set; }

        /// <summary> Gets or sets List of tasks. </summary>
        public AnalyzeDocumentsJobResult Tasks { get; set; }

        /// <summary> Gets or sets if showStats=true was specified in the request this field will contain information about the request payload. </summary>
        public RequestStatistics Statistics { get; set; }
    }
}
