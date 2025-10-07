// Copyright (c) Microsoft Corporation.
//  Licensed under the MIT License.

using System.ComponentModel;
using Azure.AI.Language.Conversations;
using Azure.AI.Language.MCP.Common.Logger;
using Azure.AI.Language.MCP.Common.Response;
using Azure.AI.Language.MCP.Common.Settings;
using Azure.AI.Language.MCP.Server.Clients.Language.Conversations;
using Azure.AI.Language.MCP.Server.Clients.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;

namespace Azure.AI.Language.MCP.Server.Language.Tools
{
    /// <summary>
    /// Mcp tool for conversational understanding using Azure.AI.Language.Conversations APIs.
    /// </summary>
    [McpServerToolType]
    public sealed class ConversationalUnderstandingTool
    {
        private const string Description = "This tool detects intents and entities in conversations. " +
            "This tools requires the message for which we need to detect the entities and find relevant entities." +
            "Results are returned as a JSON representation containing the top intent and entities. " +
            "Return the response as is and don't add additional entities.";

        private readonly ILogger<ConversationalUnderstandingTool> logger;
        private readonly ConversationAnalysisClient client;

        private readonly string projectName;
        private readonly string deploymentName;

        public ConversationalUnderstandingTool(
            ILogger<ConversationalUnderstandingTool> logger,
            ConversationAnalysisClient client,
            IOptions<AzureAIResourceSettings> config)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(config.Value?.CLUSettings?.ProjectName);
            ArgumentNullException.ThrowIfNull(config.Value?.CLUSettings?.DeploymentName);

            this.logger = logger ?? new ConsoleLogger<ConversationalUnderstandingTool>();
            this.client = client;
            this.projectName = config.Value.CLUSettings.ProjectName;
            this.deploymentName = config.Value.CLUSettings.DeploymentName;
        }

        [McpServerTool(Title = "Detects intent for text tool")]
        [Description(Description)]
        public async Task<string> DetectIntent(
            [Description("The text that needs to be used for analysis")] string message)
        {
            try
            {
                var input = AnalyzeConversationsClientHelper.CreateCLUInput(message, this.projectName, this.deploymentName);
                var response = await this.client.AnalyzeConversationAsync(input);

                var serializedResponse = response.Value.Serialize();
                return new McpResponse(serializedResponse).Serialize();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in conversational understanding: {Message}", ex.Message);
                return new McpResponse(ex.Message, isError: true).Serialize();
            }
        }
    }
}
