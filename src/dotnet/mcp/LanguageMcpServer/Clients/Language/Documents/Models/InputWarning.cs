// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models
{
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
