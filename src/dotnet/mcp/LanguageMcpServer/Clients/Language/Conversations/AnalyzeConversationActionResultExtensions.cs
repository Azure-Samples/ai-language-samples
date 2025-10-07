// Copyright (c) Microsoft Corporation.
//  Licensed under the MIT License.

using Azure.AI.Language.Conversations.Models;
using Newtonsoft.Json;
using static Azure.AI.Language.MCP.Server.Utils.DefaultSettings;

namespace Azure.AI.Language.MCP.Server.Clients.Language.Conversations
{
    /// <summary>
    /// Provides extension methods for <see cref="AnalyzeConversationActionResult"/> to support serialization.
    /// </summary>
    internal static class AnalyzeConversationActionResultExtensions
    {
        /// <summary>
        /// Serializes the <see cref="AnalyzeConversationActionResult"/> to a JSON string using the default serializer settings.
        /// If the response is null, returns an empty string.
        /// If the response is a <see cref="ConversationActionResult"/> and its prediction is a <see cref="ConversationPrediction"/>, serializes the prediction.
        /// Otherwise, throws <see cref="NotImplementedException"/>.
        /// </summary>
        /// <param name="response">The <see cref="AnalyzeConversationActionResult"/> to serialize.</param>
        /// <returns>A JSON string representation of the prediction, or an empty string if the response is null.</returns>
        /// <exception cref="NotImplementedException">Thrown if the response type or prediction type is not supported.</exception>
        public static string Serialize(this AnalyzeConversationActionResult response)
        {
            if (response == null)
            {
                return string.Empty;
            }

            if (response is ConversationActionResult conversationActionResult)
            {
                if (conversationActionResult.Result.Prediction is ConversationPrediction prediction)
                {
                    return JsonConvert.SerializeObject(prediction, DefaultJsonSerializerSettings);
                }
            }

            throw new NotImplementedException();
        }
    }
}
