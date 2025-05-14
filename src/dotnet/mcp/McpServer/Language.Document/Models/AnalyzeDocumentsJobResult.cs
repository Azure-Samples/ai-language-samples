// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Language.Document.Models
{
    /// <summary>
    /// Represents the result of an analyze documents job, including task counts and individual task results.
    /// </summary>
    public class AnalyzeDocumentsJobResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyzeDocumentsJobResult"/> class.
        /// </summary>
        /// <param name="totalTasksCount">The total number of tasks in the job.</param>
        /// <param name="completedTasksCount">The number of tasks that have been completed.</param>
        /// <param name="failedTasksCount">The number of tasks that have failed.</param>
        /// <param name="inProgressTasksCount">The number of tasks that are currently in progress.</param>
        public AnalyzeDocumentsJobResult(
            int totalTasksCount,
            int completedTasksCount,
            int failedTasksCount,
            int inProgressTasksCount)
        {
            TotalTasksCount = totalTasksCount;
            CompletedTasksCount = completedTasksCount;
            FailedTasksCount = failedTasksCount;
            InProgressTasksCount = inProgressTasksCount;
        }

        /// <summary>
        /// Gets or sets the number of tasks that have been completed.
        /// </summary>
        [JsonProperty(PropertyName = "completed")]
        public int CompletedTasksCount { get; set; }

        /// <summary>
        /// Gets or sets the number of tasks that have failed.
        /// </summary>
        [JsonProperty(PropertyName = "failed")]
        public int FailedTasksCount { get; set; }

        /// <summary>
        /// Gets or sets the number of tasks that are currently in progress.
        /// </summary>
        [JsonProperty(PropertyName = "inProgress")]
        public int InProgressTasksCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of tasks in the job.
        /// </summary>
        [JsonProperty(PropertyName = "total")]
        public int TotalTasksCount { get; set; }

        /// <summary>
        /// Gets or sets the list of individual task results for the job.
        /// </summary>
        [JsonProperty(PropertyName = "items")]
        public IList<AnalyzeDocumentsJobTaskResult> Items { get; set; }
    }
}
