// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models
{
    /// <summary>
    /// Enum representing the kinds of document locations.
    /// </summary>
    public enum DocumentLocationKind
    {
        /// <summary>
        /// No specific location type.
        /// </summary>
        None = 0,

        /// <summary>
        /// The document is located in an Azure Blob storage.
        /// </summary>
        AzureBlob,
    }
}
