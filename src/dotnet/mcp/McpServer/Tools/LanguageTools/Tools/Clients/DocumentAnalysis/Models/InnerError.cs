// Copyright (c) Microsoft Corporation.  
// Licensed under the MIT License.  

namespace LanguageAgentTools.Tools.Clients.DocumentAnalysis.Models
{
    /// <summary>
    /// Represents an error with additional details for debugging or logging purposes.
    /// </summary>
    public class InnerError
    {
        /// <summary>
        /// Gets or sets the error code that identifies the type of error.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the detailed error message providing more context about the error.
        /// </summary>
        public string Message { get; set; }
    }
}
