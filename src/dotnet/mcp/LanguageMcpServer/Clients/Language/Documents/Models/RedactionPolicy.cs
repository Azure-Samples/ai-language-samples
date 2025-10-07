// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models
{
    /// <summary>
    /// Represents a redaction policy used for masking sensitive information in documents.
    /// </summary>
    public class RedactionPolicy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedactionPolicy"/> class.
        /// </summary>
        /// <param name="kind">The kind of redaction policy to apply.</param>
        /// <param name="redactionCharacter">The character to use for masking, if applicable.</param>
        public RedactionPolicy(string policyKind, char? redactionCharacter = default)
        {
            if (Enum.TryParse<RedactionPolicyKind>(policyKind, true, out var kindEnum))
            {
                this.PolicyKind = kindEnum;
            }
            else
            {
                this.PolicyKind = RedactionPolicyKind.EntityMask;
            }

            if (this.PolicyKind == RedactionPolicyKind.CharacterMask)
            {
                redactionCharacter ??= '*';
                this.RedactionCharacter = redactionCharacter.Value;
            }
        }

        [JsonConstructor]
        public RedactionPolicy(RedactionPolicyKind policyKind, char redactionCharacter = '*')
        {
            if (policyKind == RedactionPolicyKind.CharacterMask)
            {
                this.RedactionCharacter = redactionCharacter;
            }
        }

        /// <summary>
        /// Gets or Sets the kind of the redaction policy.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public RedactionPolicyKind PolicyKind { get; set; }

        /// <summary>
        /// Gets or Sets the character used for masking in the redaction policy.
        /// </summary>
        public char RedactionCharacter { get; set; }
    }
}
