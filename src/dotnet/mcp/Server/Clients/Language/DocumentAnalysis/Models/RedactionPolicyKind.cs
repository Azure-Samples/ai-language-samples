// Copyright (c) Microsoft Corporation.  
// Licensed under the MIT License.  

namespace Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models
{
    /// <summary>
    /// Specifies the kind of redaction policy to apply.
    /// </summary>
    public enum RedactionPolicyKind
    {
        /// <summary>
        /// Redacts content by replacing characters with a mask.
        /// </summary>
        CharacterMask = 0,

        /// <summary>
        /// Redacts content by masking entire entities.
        /// </summary>
        EntityMask = 1,

        /// <summary>
        /// No redaction is applied.
        /// </summary>
        NoMask = 2,
    }
}
