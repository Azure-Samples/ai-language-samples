// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.AI.Language.Text;

namespace Language.Document.Models
{
    public class AnalyzeDocumentsResult
    {
        public IReadOnlyList<AnalyzeDocumentsTaskError> Errors { get; set; }

        /// <summary> if showStats=true was specified in the request this field will contain information about the request payload. </summary>
        public RequestStatistics Statistics { get; set; }

        /// <summary> This field indicates which model is used for analysis. </summary>
        public string ModelVersion { get; set; }

        /// <summary> Response by document. </summary>
        public IReadOnlyList<DocumentResult> Documents { get; set; }
    }
}
