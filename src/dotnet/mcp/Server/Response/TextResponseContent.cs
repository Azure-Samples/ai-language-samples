// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;
using Azure.AI.Language.MCP.Server.Response;

namespace Azure.AI.Language.MCP.Server
{
    /// <summary>
    /// Represents a text-based response content.
    /// </summary>
    public class TextResponseContent : IResponseContent
    {
        /// <summary>
        /// Gets or sets the type of the response content.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; } = "text";

        /// <summary>
        /// Gets or sets the text content of the response.
        /// </summary>
        /// 
        [JsonPropertyName("text")]
        public string Text { get; set; }

        public TextResponseContent(string text)
        {
            Text = text;
        }
    }
}