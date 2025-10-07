// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.AI.Language.MCP.Server.Clients.Language.Documents.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models
{
    /// <summary>
    /// Represents the result of a document analysis job task.
    /// </summary>
    public class AnalyzeDocumentsJobTaskResult
    {
        /// <summary>
        /// Gets or sets the kind of the task.
        /// </summary>
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; }

        /// <summary>
        /// Gets or sets the name of the task.
        /// </summary>
        [JsonProperty(PropertyName = "taskName")]
        public string TaskName { get; set; }

        /// <summary>
        /// Gets or sets the last updated date and time of the task.
        /// This property is required.
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "lastUpdateDateTime")]
        public DateTime LastUpdatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the status of the task.
        /// This property is required and uses a string enum converter.
        /// </summary>
        [JsonProperty(Required = Required.Always, PropertyName = "status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public AnalyzeJobTaskStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the results of the task.
        /// The type of this property is dynamic to allow flexibility in result representation.
        /// </summary>
        [JsonProperty(PropertyName = "results")]
        public AnalyzeDocumentsResult Results { get; set; }
    }
}
