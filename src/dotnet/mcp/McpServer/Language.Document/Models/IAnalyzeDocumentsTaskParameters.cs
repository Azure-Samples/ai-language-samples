// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;

namespace Language.Document.Models
{
    /// <summary>
    /// Represents the parameters required for analyzing documents.
    /// </summary>
    public interface IAnalyzeDocumentsTaskParameters
    {
        /// <summary>
        /// Gets the version of the model to be used for document analysis.
        /// Defaults to "latest" if not specified.
        /// </summary>
        [DefaultValue("latest")]
        public string ModelVersion { get; }
    }
}
