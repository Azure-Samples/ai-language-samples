// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models;
using Azure.AI.Language.MCP.Server.Clients.Language.Models;
using Azure.AI.Language.Text;

namespace Azure.AI.Language.MCP.Server.Clients.Language
{
    /// <summary>
    /// Interface for language client that provides methods for analyzing documents and text.
    /// </summary>
    public interface ILanguageClient
    {
        /// <summary>
        /// Asynchronously analyzes the input text for language text operations and returns the result.
        /// </summary>
        /// <param name="analyzeTextInput">Input to be analyzed.</param>
        /// <param name="showStatistics">Flag to show statistics.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>A task that represents asynchronous operation. The task result contains the analysis result as a JSON string.</returns>
        Task<AnalyzeTextResult> AnalyzeTextAsync(AnalyzeTextInput analyzeTextInput, bool? showStatistics = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Analyzes a set of text operations asynchronously and returns the state of the analysis.
        /// </summary>
        /// <param name="operationsInput">The input containing the text operations to be analyzed.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the state of the text analysis
        /// operation.</returns>
        Task<AnalyzeTextOperationState> AnalyzeTextAsync(AnalyzeTextOperationsInput operationsInput, CancellationToken cancellationToken = default);

        /// <summary>
        /// Submits a document analysis operation for document analysis and polls for the result.
        /// </summary>
        /// <param name="input">The input containing the documents and tasks for analysis.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the analysis result as a JSON string.</returns>
        /// <exception cref="RequestFailedException">Thrown when the request fails with an error response.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the result cannot be deserialized.</exception>
        Task<AnalyzeDocumentsJobState> AnalyzeDocumentAsync(IAnalyzeDocumentsInput input, CancellationToken cancellationToken = default);
    }
}