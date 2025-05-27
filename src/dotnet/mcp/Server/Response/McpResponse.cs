// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Azure.AI.Language.MCP.Server.Response
{
    public class McpResponse
    {
        /// <summary>
        /// The flag to show if the response is an error or not.
        /// </summary>
        [JsonProperty("isError")]
        public bool IsError { get; set; }

        /// <summary>
        /// Contains the content of the response.
        /// </summary>
        [JsonProperty("content")]
        [JsonConverter(typeof(ResponseContentConverter))]
        public IResponseContent Content { get; set; }

        public McpResponse(string content, bool isError = false)
        {
            IsError = isError;
            Content = new TextResponseContent(content);
        }

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
