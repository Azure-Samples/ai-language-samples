// Copyright (c) Microsoft Corporation.
//  Licensed under the MIT License.

using System.ComponentModel;
using Azure.AI.Language.MCP.Common.Response;
using Azure.AI.Language.MCP.Common.Settings;
using Azure.AI.Language.QuestionAnswering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;
using Newtonsoft.Json;

namespace Azure.AI.Language.MCP.Server.Language.Tools
{
    /// <summary>
    /// This class provides methods for question answering using Azure.AI.Language QuestionAnswering APIs.
    /// </summary>
    [McpServerToolType]
    public sealed class QuestionAnsweringTool
    {
        private const string Description = "This tool will answers any question using the configured knowledge bases." +
            "This tool requires the questionText. It optionally accepts the maximum number of answers to return, and the minimum confidence score threshold, and the ranker type." +
            "After execution it will return the response configured in a json structure. Return the response as is.";

        private readonly ILogger<QuestionAnsweringTool> logger;
        private readonly QuestionAnsweringClient client;

        private readonly string projectName;
        private readonly string deploymentName;

        public QuestionAnsweringTool(
            ILogger<QuestionAnsweringTool> logger,
            QuestionAnsweringClient client,
            IOptions<AzureAIResourceSettings> config)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(config.Value?.QuestionAnsweringSettings?.ProjectName);
            ArgumentNullException.ThrowIfNull(config.Value?.QuestionAnsweringSettings?.DeploymentName);

            this.logger = logger;
            this.client = client;
            this.projectName = config.Value.QuestionAnsweringSettings.ProjectName;
            this.deploymentName = config.Value.QuestionAnsweringSettings.DeploymentName;
        }

        [McpServerTool(Title = "Answers the specified question using your knowledge base.")]
        [Description(Description)]
        public async Task<string> GetAnswersForQuestions(
            [Description("Query string against the knowledge base.")] string message,
            [Description("(Optional)Max number of answers to be returned for the question. Default to 5")] int top = 5,
            [Description("(Optional) Minimum threshold score for answers, value ranges from 0 to 1. Default value is 0.6")] double confidenceScoreThreshold = 0.6,
            [Description("(Optional) Answer based on the question only, default false")] bool questionOnly = false)
        {
            try
            {
                var project = new QuestionAnsweringProject(this.projectName, this.deploymentName);

                RankerKind rankerType = questionOnly ? RankerKind.QuestionOnly : RankerKind.Default;

                var answersOptions = new AnswersOptions
                {
                    ConfidenceThreshold = confidenceScoreThreshold,
                    RankerKind = rankerType,
                    Size = top,
                };

                Response<AnswersResult> response = await this.client.GetAnswersAsync(message, project, answersOptions);

                string serializedResponse = JsonConvert.SerializeObject(response.Value.Answers);

                return new McpResponse(serializedResponse).Serialize();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in QuestionAnsweringTool: {Message}", ex.Message);
                return new McpResponse(ex.Message, isError: true).Serialize();
            }
        }
    }
}
