// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace LanguageAgentTools.Tools.Clients.DocumentAnalysis.Models
{
    public class AnalyzeDocumentsTaskError
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("error")]
        public InnerError Error { get; set; }
    }
}