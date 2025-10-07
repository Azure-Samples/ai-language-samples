// Copyright (c) Microsoft Corporation.
//  Licensed under the MIT License.

using Azure.AI.Language.Conversations.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StringIndexType = Azure.AI.Language.Conversations.Models.StringIndexType;

namespace Azure.AI.Language.MCP.Server.Clients.Utils
{
    internal static class AnalyzeConversationsClientHelper
    {
        private const string DefaultInputId = "1";

        public static AnalyzeConversationInput CreateCLUInput(
            string message,
            string projectName,
            string deploymentName)
        {
            AnalyzeConversationInput input = new ConversationLanguageUnderstandingInput(
                new ConversationAnalysisInput(
                    new TextConversationItem(
                        id: "1",
                        participantId: "participant1",
                        text: message)),
                new ConversationLanguageUnderstandingActionContent(projectName, deploymentName)
                {
                    // Use Utf16CodeUnit for strings in .NET.
                    StringIndexType = StringIndexType.Utf16CodeUnit,
                });
            return input;
        }
    }
}
