// Copyright (c) Microsoft Corporation.
//  Licensed under the MIT License.

using Azure.AI.Language.Text;
using Newtonsoft.Json;
using static Azure.AI.Language.MCP.Server.Utils.DefaultSettings;

namespace Azure.AI.Language.MCP.Server.Clients.Language.Text
{
    /// <summary>
    /// Provides extension methods for <see cref="AnalyzeTextOperationState"/> to support serialization.
    /// </summary>
    public static class AnalyzeTextOperationStateExtensions
    {
        /// <summary>
        /// Serializes the <see cref="AnalyzeTextOperationState"/> or its result to a JSON string.
        /// If the response is null, returns an empty string.
        /// If the response status is not <see cref="TextActionState.Succeeded"/>, serializes the entire response.
        /// If the response contains a known operation result, serializes that result.
        /// </summary>
        /// <param name="response">The <see cref="AnalyzeTextOperationState"/> to serialize.</param>
        /// <returns>A JSON string representation of the response or its result.</returns>
        public static string Serialize(this AnalyzeTextOperationState response)
        {
            if (response == null)
            {
                return string.Empty;
            }

            if (response.Status != TextActionState.Succeeded)
            {
                return JsonConvert.SerializeObject(response, DefaultJsonSerializerSettings);
            }

            // return the first item in the Actions.Items collection
            return JsonConvert.SerializeObject(response.Actions.Items[0], DefaultJsonSerializerSettings);
        }
    }
}
