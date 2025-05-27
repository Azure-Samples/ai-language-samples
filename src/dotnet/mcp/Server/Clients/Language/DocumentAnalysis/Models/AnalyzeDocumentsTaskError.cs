// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models
{
    public class AnalyzeDocumentsTaskError
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("error")]
        public InnerError Error { get; set; }
    }
}