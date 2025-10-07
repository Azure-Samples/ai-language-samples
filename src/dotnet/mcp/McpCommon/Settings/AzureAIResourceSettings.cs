// Copyright (c) Microsoft Corporation. 
//  Licensed under the MIT License.

namespace Azure.AI.Language.MCP.Common.Settings
{
    /// <summary>
    /// Represents the configuration settings required to connect to the Azure AI Language service.
    /// </summary>
    public class AzureAIResourceSettings
    {
        /// <summary>
        /// Gets or sets the endpoint URL for the Azure AI Language service.
        /// </summary>
        public string Endpoint { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the API key used to authenticate requests to the Azure AI Language service.
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Azure region where the Language service is hosted.
        /// </summary>
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the project settings for Question Answering.
        /// </summary>
        public ProjectSettings? QuestionAnsweringSettings { get; set; }

        /// <summary>
        /// Gets or sets the project settings for Conversational Language Understanding (CLUSettings).
        /// </summary>
        public ProjectSettings? CLUSettings { get; set; }
    }
}