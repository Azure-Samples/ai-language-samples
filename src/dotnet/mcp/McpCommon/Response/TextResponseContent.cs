// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Azure.AI.Language.MCP.Common.Response
{
    /// <summary>
    /// Represents a text-based response content.
    /// </summary>
    public class TextResponseContent : IResponseContent
    {
        public TextResponseContent(string text)
        {
            Text = text;
        }

        /// <summary>
        /// Gets or sets the type of the response content.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; } = "text";

        /// <summary>
        /// Gets or sets the text content of the response.
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}