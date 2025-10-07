// Copyright (c) Microsoft Corporation.
//  Licensed under the MIT License.

using Azure.AI.Language.Text;

namespace Azure.AI.Language.MCP.Server.Clients.Language.Models
{
    /// <summary>
    /// Represents the input for an analyze text operation, including the text and the action to perform.
    /// </summary>
    public class AnalyzeTextOperationsInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyzeTextOperationsInput"/> class.
        /// </summary>
        /// <param name="multiLanguageInput">The multi-language text input to analyze.</param>
        /// <param name="action">The action to perform on the input text.</param>
        public AnalyzeTextOperationsInput(MultiLanguageTextInput multiLanguageInput, AnalyzeTextOperationAction action)
        {
            this.Input = multiLanguageInput;
            this.Action = action;
        }

        /// <summary>
        /// Gets or sets the multi-language text input to analyze.
        /// </summary>
        public MultiLanguageTextInput Input { get; set; }

        /// <summary>
        /// Gets or sets the action to perform on the input text.
        /// </summary>
        public AnalyzeTextOperationAction Action { get; set; }
    }
}
