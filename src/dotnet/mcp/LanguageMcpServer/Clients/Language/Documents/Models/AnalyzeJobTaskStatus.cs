// Copyright (c) Microsoft Corporation.
//  Licensed under the MIT License.

namespace Azure.AI.Language.MCP.Server.Clients.Language.Documents.Models
{
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
        Failed,
    }
}