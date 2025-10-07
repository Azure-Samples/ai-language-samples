// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Azure.AI.Language.MCP.Common.Response
{
    /// <summary>
    /// Represents a response from the MCP server, containing content and an error indicator.
    /// </summary>
    public class McpResponse
    {
        public McpResponse(string content, bool isError = false)
        {
            IsError = isError;
            Content = new TextResponseContent(content);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show if the response is an error or not.
        /// </summary>
        [JsonProperty("isError")]
        public bool IsError { get; set; }

        /// <summary>
        /// Gets or sets the content of the response.
        /// </summary>
        [JsonProperty("content")]
        [JsonConverter(typeof(ResponseContentConverter))]
        public IResponseContent Content { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            });
        }
    }
}
