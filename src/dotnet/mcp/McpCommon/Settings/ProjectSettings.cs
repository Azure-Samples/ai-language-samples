// Copyright (c) Microsoft Corporation. 
//  Licensed under the MIT License.

namespace Azure.AI.Language.MCP.Common.Settings
{
    /// <summary>
    /// Represents the settings for a project, including project and deployment names.
    /// </summary>
    public class ProjectSettings
    {
        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        public string ProjectName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the deployment.
        /// </summary>
        public string DeploymentName { get; set; } = string.Empty;
    }
}