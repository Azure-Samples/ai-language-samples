// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Language.Document.Models
{
    /// <summary>
    /// Represents the input required for document analysis.
    /// </summary>
    public interface IAnalyzeDocumentsInput
    {
        /// <summary>
        /// Gets the kind of document analysis task to be performed.
        /// </summary>
        AnalyzeDocumentsTaskKind Kind { get; }

        /// <summary>
        /// Gets the input data for the analysis, supporting multiple languages.
        /// </summary>
        AnalyzeDocumentsInput AnalysisInput { get; }

        /// <summary>
        /// Gets the list of tasks to be executed during the document analysis.
        /// </summary>
        IList<AnalyzeDocumentsTask> Tasks { get; }
    }
}
