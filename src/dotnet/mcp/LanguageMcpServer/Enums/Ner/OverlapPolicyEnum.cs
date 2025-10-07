// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Azure.AI.Language.MCP.Server.Enums.Ner
{
    /// <summary>
    /// Overlap policy for named entity recognition.
    /// </summary>
    public enum OverlapPolicyEnum
    {
        MatchLongest = 0,
        AllowOverlap = 1,
    }
}
