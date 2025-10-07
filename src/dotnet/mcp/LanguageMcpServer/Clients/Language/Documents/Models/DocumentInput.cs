// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Text.Json.Serialization;

namespace Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models
{
    /// <summary>
    /// Represents the input for a document processing operation.
    /// </summary>
    public class DocumentInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentInput"/> class.
        /// </summary>
        /// <param name="id">The unique identifier for the document.</param>
        /// <param name="source">The source location of the document.</param>
        /// <param name="target">The target location of the document.</param>
        /// <param name="language">The language of the document.</param>
        [JsonConstructor]
        public DocumentInput(string id, DocumentLocation source, DocumentLocation target, string language)
        {
            this.Id = id;
            this.Source = source;
            this.Target = target;
            this.Language = language;
        }

        /// <summary>
        /// Gets the unique identifier for the document.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the source location of the document.
        /// </summary>
        public DocumentLocation Source { get; }

        /// <summary>
        /// Gets the target location of the document.
        /// </summary>
        public DocumentLocation Target { get; }

        /// <summary>
        /// Gets or sets the language of the document.
        /// </summary>
        public string Language { get; set; }
    }
}
