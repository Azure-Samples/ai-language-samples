// Copyright (c) Microsoft Corporation.
//  Licensed under the MIT License.

using Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models;
using Newtonsoft.Json;
using static Azure.AI.Language.MCP.Server.Utils.DefaultSettings;

namespace Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis
{
    public static class AnalyzeDocumentsJobStateExtensions
    {
        /// <summary>
        /// Converts the API response from document PII entity recognition to a JSON string suitable for MCP response.
        /// </summary>
        /// <param name="response">The SDK response of type <see cref="AnalyzeDocumentsJobState"/>.</param>
        /// <returns>A JSON string representing the result or errors.</returns>
        public static string Serialize(this AnalyzeDocumentsJobState response)
        {
            if (response.Error != null)
            {
                return JsonConvert.SerializeObject(response.Error, DefaultJsonSerializerSettings);
            }

            if (response.Errors?.Any() ?? false)
            {
                return JsonConvert.SerializeObject(response.Errors, DefaultJsonSerializerSettings);
            }

            var errorsInResponse = response.Tasks.Items.Where(task => task.Results?.Errors?.Any() ?? false);

            if (errorsInResponse?.Any() ?? false)
            {
                return JsonConvert.SerializeObject(errorsInResponse.SelectMany(task => task.Results.Errors));
            }

            return JsonConvert.SerializeObject(response.Tasks.Items.SelectMany(task => task.Results.Documents));
        }
    }
}
