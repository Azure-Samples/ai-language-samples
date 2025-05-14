// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Language.Document.Models
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


    /// <summary>
    /// Represents the possible statuses of an analysis job task.
    /// </summary>
    public enum AnalyzeJobTaskStatus
    {
        /// <summary>
        /// The status of the task is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The task has not started yet.
        /// </summary>
        NotStarted,

        /// <summary>
        /// The task is currently running.
        /// </summary>
        Running,

        /// <summary>
        /// The task is in the process of being canceled.
        /// </summary>
        Cancelling,

        /// <summary>
        /// The task has been canceled.
        /// </summary>
        Cancelled,

        /// <summary>
        /// The task has completed successfully.
        /// </summary>
        Succeeded,

        /// <summary>
        /// The task has failed.
        /// </summary>
        Failed
    }
}
