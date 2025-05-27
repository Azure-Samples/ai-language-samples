// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.AI.Language.Text;
using Azure;
using Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models;

namespace Azure.AI.Language.MCP.Server.Clients.Language
{
    /// <summary>
    /// Interface for language client that provides methods for analyzing documents and text.
    /// </summary>
    public interface ILanguageClient
    {
        Task<AnalyzeDocumentsJobState> AnalyzeDocumentAsync(DocumentPiiEntityRecognitionInput input, CancellationToken cancellationToken = default);

        Task<AnalyzeTextResult> AnalyzeAsync(AnalyzeTextInput analyzeTextInput, bool? showStatistics = null, CancellationToken cancellationToken = default);
    }
}