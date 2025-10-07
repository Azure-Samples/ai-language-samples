// Copyright (c) Microsoft Corporation.
//  Licensed under the MIT License.

using Azure.AI.Language.Text;
using Newtonsoft.Json;
using static Azure.AI.Language.MCP.Server.Utils.DefaultSettings;

namespace Azure.AI.Language.MCP.Server.Clients.Language.Text
{
    internal static class AnalyzeTextResultExtensions
    {
        /// <summary>
        /// Converts the API response from analyze-text API to a JSON string suitable for MCP response.
        /// </summary>
        /// <param name="response">The SDK response of type <see cref="AnalyzeTextResult"/>.</param>
        /// <returns>A JSON string representing the result or errors.</returns>
        public static string Serialize(this AnalyzeTextResult response)
        {
            if (response == null)
            {
                return string.Empty;
            }

            return response switch
            {
                AnalyzeTextPiiResult piiResult => SerializeResult(piiResult.Results),
                AnalyzeTextEntitiesResult entitiesResult => SerializeResult(entitiesResult.Results),
                AnalyzeTextSentimentResult sentimentResult => SerializeResult(sentimentResult.Results),
                AnalyzeTextKeyPhraseResult keyPhraseResult => SerializeResult(keyPhraseResult.Results),
                AnalyzeTextLanguageDetectionResult languageDetectionResult => SerializeResult(languageDetectionResult.Results),
                _ => throw new NotImplementedException()
            };

            static string SerializeResult(dynamic results)
            {
                if ((results?.Errors as IEnumerable<object>)?.Any() == true)
                {
                    return JsonConvert.SerializeObject(results?.Errors, DefaultJsonSerializerSettings);
                }

                return JsonConvert.SerializeObject(results?.Documents, DefaultJsonSerializerSettings);
            }
        }
    }
}
