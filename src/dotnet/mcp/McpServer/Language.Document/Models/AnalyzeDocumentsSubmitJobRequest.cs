// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Newtonsoft.Json;

namespace Language.Document.Models
{
    /// <summary>
    /// Represents a request to submit a job for analyzing documents.
    /// </summary>
    internal class AnalyzeDocumentsSubmitJobRequest
    {
        /// <summary>
        /// Gets the input data for the analysis, which includes the documents to be analyzed.
        /// </summary>
        public AnalyzeDocumentsInput AnalysisInput { get; }

        /// <summary>
        /// Gets the list of tasks to be performed on the documents.
        /// </summary>
        public IReadOnlyList<AnalyzeDocumentsTask> Tasks { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyzeDocumentsSubmitJobRequest"/> class.
        /// </summary>
        /// <param name="analysisInput">The input data for the analysis.</param>
        /// <param name="tasks">The tasks to be performed on the documents.</param>
        internal AnalyzeDocumentsSubmitJobRequest(AnalyzeDocumentsInput analysisInput, IEnumerable<AnalyzeDocumentsTask> tasks)
        {
            AnalysisInput = analysisInput;
            Tasks = tasks.ToList();
        }

        /// <summary>
        /// Converts the current instance into a <see cref="RequestContent"/> object
        /// that can be sent as part of an HTTP request.
        /// </summary>
        /// <returns>A <see cref="RequestContent"/> object containing the serialized request data.</returns>
        public RequestContent ToRequestContent()
        {
            var json = JsonConvert.SerializeObject(this, formatting: Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            return RequestContent.Create(System.Text.Encoding.UTF8.GetBytes(json));
        }
    }
}
