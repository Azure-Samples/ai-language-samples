// Copyright (c) Microsoft Corporation.
//  Licensed under the MIT License.

using Azure.AI.Language.MCP.Server.Utils.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Azure.AI.Language.MCP.Server.Utils
{
    /// <summary>
    /// Provides default JSON serializer settings for the server, including custom converters and property naming policies.
    /// </summary>
    public static class DefaultSettings
    {
        /// <summary>
        /// Gets default <see cref="JsonSerializerSettings"/> used for serializing <see cref="AnalyzeTextOperationState"/> and related results.
        /// </summary>
        public static JsonSerializerSettings DefaultJsonSerializerSettings { get; } = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter>
                {
                    new ScriptConverter(),
                    new ScriptCodeConverter(),
                },
        };
    }
}
