// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models
{
    /// <summary>
    /// Represents a task for analyzing documents.
    /// </summary>
    public class AnalyzeDocumentsTask
    {
        /// <summary>
        /// Gets or sets the kind of the analyze documents task.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public AnalyzeDocumentsTaskKind Kind { get; set; }

        /// <summary>
        /// Gets or sets the parameters for the analyze documents task.
        /// </summary>
        public IAnalyzeDocumentsTaskParameters Parameters { get; set; }
    }
}
