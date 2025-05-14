// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Language.Document.Models
{
    /// <summary>
    /// Represents the types of tasks that can be performed to analyze documents.
    /// </summary>
    public enum AnalyzeDocumentsTaskKind
    {
        /// <summary>
        /// Task for recognizing Personally Identifiable Information (PII) entities in documents.
        /// </summary>
        PiiEntityRecognition,
    }
}