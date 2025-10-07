// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models
{
    /// <summary>
    /// Represents the input for analyzing multiple language documents.
    /// </summary>
    public class AnalyzeDocumentsInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyzeDocumentsInput"/> class.
        /// This constructor initializes the <see cref="Documents"/> property with an empty list.
        /// </summary>
        public AnalyzeDocumentsInput()
        {
            this.Documents = new List<DocumentInput>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyzeDocumentsInput"/> class
        /// with a specified list of document inputs. This constructor is primarily used
        /// for JSON deserialization.
        /// </summary>
        /// <param name="multiLanguageInputs">A list of document inputs to initialize the <see cref="Documents"/> property.</param>
        [JsonConstructor]
        public AnalyzeDocumentsInput(IList<DocumentInput> multiLanguageInputs)
        {
            // Assign the provided list of document inputs to the Documents property.
            this.Documents = multiLanguageInputs;
        }

        /// <summary>
        /// Gets the list of document inputs, representing multiple language documents.
        /// </summary>
        public IList<DocumentInput> Documents { get; }
    }
}
