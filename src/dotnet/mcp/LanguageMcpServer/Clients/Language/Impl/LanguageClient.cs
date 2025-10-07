// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models;
using Azure.AI.Language.MCP.Server.Clients.Language.Models;
using Azure.AI.Language.Text;
using Azure.Core;
using Newtonsoft.Json;

namespace Azure.AI.Language.MCP.Server.Clients.Language
{
    /// <summary>
    /// A client for performing text and document analysis operations, such as PII entity recognition.
    /// </summary>
    public class LanguageClient : TextAnalysisClient, ILanguageClient
    {
        private const string ApiVersion = "2024-11-15-preview";
        private const int DefaultTimeoutInSeconds = 300;

        private static readonly string[] FailureStates = { "failed", "canceled" };
        private static readonly string[] SuccessStates = { "succeeded" };

        private static RequestContext defaultRequestContext = new RequestContext();
        private readonly string apiVersion;
        private readonly Uri endpoint;
        private readonly TimeSpan timeoutForJobs;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageClient"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint URI for the Azure Language service.</param>
        /// <param name="credential">The credential used to authenticate with the service.</param>
        /// <param name="options">The client options for configuring the service client.</param>
        public LanguageClient(Uri endpoint, AzureKeyCredential credential, TextAnalysisClientOptions options, int jobTimeoutInSeconds = default)
            : base(endpoint, credential, options)
        {
            this.apiVersion = ApiVersion;
            this.endpoint = endpoint;

            if (jobTimeoutInSeconds <= 0)
            {
                jobTimeoutInSeconds = DefaultTimeoutInSeconds;
            }

            this.timeoutForJobs = TimeSpan.FromMinutes(jobTimeoutInSeconds);
        }

        /// <summary>
        /// Asynchronously analyzes the input text for PII entity recognition and returns the result.
        /// </summary>
        /// <param name="analyzeTextInput">Input to be analyzed.</param>
        /// <param name="showStatistics">Flag to show statistics.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>A task that represents asynchronous operation. The task result contains the analysis result as a JSON string.</returns>
        public new async Task<AnalyzeTextResult> AnalyzeTextAsync(AnalyzeTextInput analyzeTextInput, bool? showStatistics = null, CancellationToken cancellationToken = default)
        {
            Response<AnalyzeTextResult> response = await base.AnalyzeTextAsync(analyzeTextInput, showStatistics, cancellationToken);
            return response.Value;
        }

        /// <summary>
        /// Asynchronously analyzes the input text for PII entity recognition and returns the result.
        /// </summary>

        /// <returns>A task that represents asynchronous operation. The task result contains the analysis result as a JSON string.</returns>
        public async Task<AnalyzeTextOperationState> AnalyzeTextAsync(
            AnalyzeTextOperationsInput operationsInput,
            CancellationToken cancellationToken = default)
        {
            List<AnalyzeTextOperationAction> actions = new List<AnalyzeTextOperationAction> { operationsInput.Action };

            Response<AnalyzeTextOperationState> response = await this.AnalyzeTextOperationAsync(operationsInput.Input, actions, cancellationToken: cancellationToken);
            return response.Value;
        }

        /// <summary>
        /// Submits a document analysis operation for PII entity recognition and polls for the result.
        /// </summary>
        /// <param name="input">The input containing the documents and tasks for analysis.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the analysis result as a JSON string.</returns>
        /// <exception cref="RequestFailedException">Thrown when the request fails with an error response.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the result cannot be deserialized.</exception>
        public async Task<AnalyzeDocumentsJobState> AnalyzeDocumentAsync(
            IAnalyzeDocumentsInput input,
            CancellationToken cancellationToken = default)
        {
            AnalyzeDocumentsSubmitJobRequest analyzeTextOperationInput = new AnalyzeDocumentsSubmitJobRequest(input.AnalysisInput, input.Tasks);
            RequestContext context = FromCancellationToken(cancellationToken);
            try
            {
                using HttpMessage message = this.CreateAnalyzeDocumentSubmitOperationRequest(analyzeTextOperationInput.ToRequestContent(), context);
                return await this.SubmitDocumentAnalysisRequestAsync(message, cancellationToken);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static bool IsFinalState(Azure.Response response, out string failureResponse)
        {
            if (response.Status == 202)
            {
                failureResponse = string.Empty;
                return false;
            }

            if (response.Status is >= 200 and <= 204)
            {
                if (response.ContentStream is { Length: > 0 })
                {
                    try
                    {
                        using JsonDocument document = JsonDocument.Parse(response.ContentStream);
                        JsonElement root = document.RootElement;
                        root.TryGetProperty("status", out JsonElement property);
                        if (property.ValueKind != JsonValueKind.String)
                        {
                            failureResponse = "Invalid response format.";
                            return true;
                        }

                        string state = property.GetString().ToLowerInvariant();

                        if (FailureStates.Contains(state))
                        {
                            failureResponse = state;
                            return true;
                        }
                        else if (!SuccessStates.Contains(state))
                        {
                            failureResponse = string.Empty;
                            return false;
                        }
                        else
                        {
                            failureResponse = string.Empty;
                            return true;
                        }
                    }
                    finally
                    {
                        // It is required to reset the position of the content after reading as this response may be used for deserialization.
                        response.ContentStream.Position = 0;
                    }
                }
            }

            failureResponse = response.ReasonPhrase;
            return true;
        }

        /// <summary>
        /// Creates a <see cref="RequestContext"/> from a cancellation token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <returns>A <see cref="RequestContext"/> representing the cancellation token.</returns>
        private static RequestContext FromCancellationToken(CancellationToken cancellationToken = default)
        {
            if (!cancellationToken.CanBeCanceled)
            {
                return defaultRequestContext;
            }

            return new RequestContext
            {
                CancellationToken = cancellationToken,
            };
        }

        private async Task<AnalyzeDocumentsJobState> SubmitDocumentAnalysisRequestAsync(
            HttpMessage message,
            CancellationToken cancellationToken)
        {
            await this.Pipeline.SendAsync(message, cancellationToken);
            Response response = message.Response;

            if (response.IsError)
            {
                return this.ToAnalyzeDocumentsOperationState(response);
            }

            // get the operation location from the response headers
            response.Headers.TryGetValue("operation-location", out string? operationLocation);

            // Step 2: Poll the operation URL
            return this.ToAnalyzeDocumentsOperationState(await this.WaitForDocumentAnalysisCompletionAsync(operationLocation));
        }

        private AnalyzeDocumentsJobState? ToAnalyzeDocumentsOperationState(Azure.Response response)
        {
            // Deserialize the JSON element into the AnalyzeDocumentsOperationState object
            string jsonString = response.Content.ToString();
            JsonDocument jsonDoc = JsonDocument.Parse(jsonString);
            if (jsonDoc.RootElement.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<AnalyzeDocumentsJobState>(jsonDoc.RootElement.GetRawText());
        }

        private async Task<Response> WaitForDocumentAnalysisCompletionAsync(string? operationLocation)
        {
            CancellationTokenSource cts = new CancellationTokenSource(this.timeoutForJobs);

            if (string.IsNullOrEmpty(operationLocation))
            {
                throw new Exception("Operation location is null or empty.");
            }

            while (true)
            {
                Response response = await this.GetResponseAsync(operationLocation, cts.Token);
                bool hasCompleted = IsFinalState(response, out string? failureState);

                if (hasCompleted)
                {
                    if (string.IsNullOrEmpty(failureState))
                    {
                        return response;
                    }
                }

                await Task.Delay(2000);
            }
        }

        private async ValueTask<Response> GetResponseAsync(string uri, CancellationToken cancellationToken)
        {
            using HttpMessage message = this.CreateRequest(uri);
            await this.Pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            return message.Response;
        }

        private HttpMessage CreateRequest(string uri)
        {
            HttpMessage message = this.Pipeline.CreateMessage();
            Request request = message.Request;
            request.Method = RequestMethod.Get;

            if (Uri.TryCreate(uri, UriKind.Absolute, out Uri? nextLink))
            {
                request.Uri.Reset(nextLink);
            }

            return message;
        }

        /// <summary>
        /// Creates an HTTP message for submitting a document analysis operation request.
        /// </summary>
        /// <param name="content">The request content to include in the message.</param>
        /// <param name="context">The request context for the operation.</param>
        /// <returns>An <see cref="HttpMessage"/> representing the request.</returns>
        private HttpMessage CreateAnalyzeDocumentSubmitOperationRequest(RequestContent content, RequestContext context)
        {
            HttpMessage httpMessage = this.Pipeline.CreateMessage(context);
            Request request = httpMessage.Request;
            request.Method = RequestMethod.Post;
            RequestUriBuilder rawRequestUriBuilder = new RequestUriBuilder();
            rawRequestUriBuilder.Reset(this.endpoint);
            rawRequestUriBuilder.AppendPath("/language/analyze-documents/jobs", escape: false);
            rawRequestUriBuilder.AppendQuery("api-version", this.apiVersion, escapeValue: true);
            request.Uri = rawRequestUriBuilder;
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Content-Type", "application/json");
            request.Content = content;
            return httpMessage;
        }
    }
}
