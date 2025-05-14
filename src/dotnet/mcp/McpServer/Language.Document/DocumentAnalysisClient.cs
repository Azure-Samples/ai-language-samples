// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Azure;
using Azure.AI.Language.Text;
using Azure.Core;
using Language.Document.Models;
using Newtonsoft.Json;

namespace Language.Document
{
    /// <summary>
    /// A client for performing document analysis operations, such as PII entity recognition.
    /// </summary>
    public class DocumentAnalysisClient : TextAnalysisClient
    {
        private const string ApiVersion = "2024-11-15-preview";
        private const int DefaultTimeoutInSeconds = 300;

        private static readonly string[] FailureStates = { "failed", "canceled" };
        private static readonly string[] SuccessStates = { "succeeded" };

        private static RequestContext DefaultRequestContext = new RequestContext();

        private readonly string _apiVersion;
        private readonly Uri _endpoint;
        private readonly TimeSpan _timeoutForJobs;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAnalysisClient"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint URI for the Azure Language service.</param>
        /// <param name="credential">The credential used to authenticate with the service.</param>
        /// <param name="options">The client options for configuring the service client.</param>
        public DocumentAnalysisClient(Uri endpoint, AzureKeyCredential credential, TextAnalysisClientOptions options, int jobTimeoutInSeconds = default)
            : base(endpoint, credential, options)
        {
            _apiVersion = ApiVersion;
            _endpoint = endpoint;

            if (jobTimeoutInSeconds <= 0)
            {
                jobTimeoutInSeconds = DefaultTimeoutInSeconds;
            }

            _timeoutForJobs = TimeSpan.FromMinutes(jobTimeoutInSeconds);
        }

        /// <summary>
        /// Submits a document analysis operation for PII entity recognition and polls for the result.
        /// </summary>
        /// <param name="input">The input containing the documents and tasks for analysis.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the analysis result as a JSON string.</returns>
        /// <exception cref="RequestFailedException">Thrown when the request fails with an error response.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the result cannot be deserialized.</exception>
        public async Task<AnalyzeDocumentsJobState> AnalyzeDocumentOperationAsync(
            DocumentPiiEntityRecognitionInput input,
            CancellationToken cancellationToken = default)
        {
            var analyzeTextOperationInput = new AnalyzeDocumentsSubmitJobRequest(input.AnalysisInput, input.Tasks);
            RequestContext context = FromCancellationToken(cancellationToken);
            try
            {
                using HttpMessage message = CreateAnalyzeDocumentSubmitOperationRequest(analyzeTextOperationInput.ToRequestContent(), context);
                return await SubmitDocumentAnalysisRequestAsync(message, cancellationToken);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<AnalyzeDocumentsJobState> SubmitDocumentAnalysisRequestAsync(
            HttpMessage message,
            CancellationToken cancellationToken)
        {
            await Pipeline.SendAsync(message, cancellationToken);
            var response = message.Response;

            if (response.IsError)
            {
                return ToAnalyzeDocumentsOperationState(response);
            }

            // get the operation location from the response headers
            response.Headers.TryGetValue("operation-location", out var operationLocation);

            // Step 2: Poll the operation URL
            return ToAnalyzeDocumentsOperationState(await WaitForDocumentAnalysisCompletionAsync(operationLocation));
        }

        private AnalyzeDocumentsJobState ToAnalyzeDocumentsOperationState(Response response)
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
            CancellationTokenSource cts = new CancellationTokenSource(_timeoutForJobs);
            while (true)
            {
                Response response = await GetResponseAsync(operationLocation, cts.Token);
                var hasCompleted = IsFinalState(response, out var failureState);

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
            using HttpMessage message = CreateRequest(uri);
            await Pipeline.SendAsync(message, cancellationToken).ConfigureAwait(false);
            return message.Response;
        }

        private HttpMessage CreateRequest(string uri)
        {
            HttpMessage message = Pipeline.CreateMessage();
            Request request = message.Request;
            request.Method = RequestMethod.Get;

            if (Uri.TryCreate(uri, UriKind.Absolute, out var nextLink))
            {
                request.Uri.Reset(nextLink);
            }
            return message;
        }

        private static bool IsFinalState(Response response, out string failureResponse)
        {
            if (response.Status == 202)
            {
                failureResponse = "";
                return false;
            }

            if (response.Status is >= 200 and <= 204)
            {
                if (response.ContentStream is { Length: > 0 })
                {
                    try
                    {
                        using JsonDocument document = JsonDocument.Parse(response.ContentStream);
                        var root = document.RootElement;
                        root.TryGetProperty("status", out JsonElement property);
                        if (property.ValueKind != JsonValueKind.String)
                        {
                            failureResponse = "Invalid response format.";
                            return true;
                        }
                        var state = property.GetString().ToLowerInvariant();

                        if (FailureStates.Contains(state))
                        {
                            failureResponse = state;
                            return true;
                        }
                        else if (!SuccessStates.Contains(state))
                        {
                            failureResponse = "";
                            return false;
                        }
                        else
                        {
                            failureResponse = "";
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
        /// Creates an HTTP message for submitting a document analysis operation request.
        /// </summary>
        /// <param name="content">The request content to include in the message.</param>
        /// <param name="context">The request context for the operation.</param>
        /// <returns>An <see cref="HttpMessage"/> representing the request.</returns>
        internal HttpMessage CreateAnalyzeDocumentSubmitOperationRequest(RequestContent content, RequestContext context)
        {
            HttpMessage httpMessage = Pipeline.CreateMessage(context);
            Request request = httpMessage.Request;
            request.Method = RequestMethod.Post;
            var rawRequestUriBuilder = new RequestUriBuilder();
            rawRequestUriBuilder.Reset(_endpoint);
            rawRequestUriBuilder.AppendPath("/language/analyze-documents/jobs", escape: false);
            rawRequestUriBuilder.AppendQuery("api-version", _apiVersion, escapeValue: true);
            request.Uri = rawRequestUriBuilder;
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Content-Type", "application/json");
            request.Content = content;
            return httpMessage;
        }

        /// <summary>
        /// Creates a <see cref="RequestContext"/> from a cancellation token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <returns>A <see cref="RequestContext"/> representing the cancellation token.</returns>
        internal static RequestContext FromCancellationToken(CancellationToken cancellationToken = default)
        {
            if (!cancellationToken.CanBeCanceled)
            {
                return DefaultRequestContext;
            }

            return new RequestContext
            {
                CancellationToken = cancellationToken
            };
        }
    }
}
