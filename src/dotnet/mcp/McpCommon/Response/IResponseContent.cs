// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Azure.AI.Language.MCP.Common.Response
{
    /// <summary>
    /// Interface representing the content of a response.
    /// </summary>
    public interface IResponseContent
    {
        /// <summary>
        /// Gets the type of the content.
        /// </summary>
        string Type { get; }
    }
}