// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models
{
    /// <summary>
    /// Represents the location of a document, including its type and physical location.
    /// </summary>
    public class DocumentLocation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentLocation"/> class.
        /// </summary>
        /// <param name="location">The physical or logical location of the document.</param>
        /// <param name="kind">The kind of document location. Defaults to <see cref="DocumentLocationKind.AzureBlob"/> if not specified or invalid.</param>
        [JsonConstructor]
        public DocumentLocation(string location, string kind = default)
        {
            if (Enum.TryParse<DocumentLocationKind>(kind, true, out var kindEnum))
            {
                this.Kind = kindEnum;
            }
            else
            {
                this.Kind = DocumentLocationKind.AzureBlob;
            }

            this.Location = location;
        }

        /// <summary>
        /// Gets the type of document location kind.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public DocumentLocationKind Kind { get; }

        /// <summary>
        /// Gets the physical or logical location of the document.
        /// </summary>
        public string Location { get; }

        /// <summary>
        /// Returns a string representation of the DocumentLocation instance.
        /// </summary>
        /// <returns>A string in the format "Kind: {Kind}, Location: {Location}".</returns>
        public override string ToString()
        {
            return $"{this.Location} ({this.Kind})";
        }
    }
}