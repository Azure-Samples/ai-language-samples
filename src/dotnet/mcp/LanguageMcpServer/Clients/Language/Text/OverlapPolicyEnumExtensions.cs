// Copyright (c) Microsoft Corporation.
//  Licensed under the MIT License.

using Azure.AI.Language.MCP.Server.Enums.Ner;
using Azure.AI.Language.Text;

namespace Azure.AI.Language.MCP.Server.Clients.Language.Text
{
    /// <summary>
    /// Extension methods for converting <see cref="OverlapPolicyEnum"/> to <see cref="EntityOverlapPolicy"/>.
    /// </summary>
    public static class OverlapPolicyEnumExtensions
    {
        /// <summary>
        /// Converts an <see cref="OverlapPolicyEnum"/> value to its corresponding <see cref="EntityOverlapPolicy"/> implementation.
        /// </summary>
        /// <param name="overlapPolicy">The overlap policy enum value to convert.</param>
        /// <returns>
        /// An instance of <see cref="EntityOverlapPolicy"/> that matches the specified <see cref="OverlapPolicyEnum"/> value.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the provided <paramref name="overlapPolicy"/> value is not supported.
        /// </exception>
        public static EntityOverlapPolicy ToOverlapPolicy(this OverlapPolicyEnum overlapPolicy)
        {
            return overlapPolicy switch
            {
                OverlapPolicyEnum.MatchLongest => new MatchLongestEntityPolicyType(),
                OverlapPolicyEnum.AllowOverlap => new AllowOverlapEntityPolicyType(),
                _ => throw new ArgumentOutOfRangeException(nameof(overlapPolicy), overlapPolicy, null),
            };
        }
    }
}
