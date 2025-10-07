// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models
{
    /// <summary>
    /// Represents an error that occurred during document analysis.
    /// </summary>
    public class AnalyzeDocumentsError
    {
        /// <summary>
        /// Gets or sets the error code that identifies the type of error.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the detailed error message providing more context about the error.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets additional details about the error for debugging or logging purposes.
        /// </summary>
        public InnerError InnerError { get; set; }
    }
}
