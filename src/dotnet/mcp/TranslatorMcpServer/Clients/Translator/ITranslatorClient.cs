// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.AI.Translation.Text;

namespace Azure.AI.Language.MCP.Server.Translator.Clients.Translator
{
    /// <summary>
    /// Interface for a client that interacts with the Azure AI Translation service to translate text.
    /// </summary>
    public interface ITranslatorClient
    {
        /// <summary>
        /// Translates the specified content into one or more target languages using the Azure AI Translation service.
        /// </summary>
        /// <param name="targetLanguages">A collection of language codes representing the target languages for translation.</param>
        /// <param name="content">A collection of strings containing the text to be translated.</param>
        /// <param name="clientTraceId">An optional trace identifier for tracking the request.</param>
        /// <param name="sourceLanguage">The language code of the source text. If null, the service will attempt to auto-detect the language.</param>
        /// <param name="textType">Specifies whether the input text is plain text or HTML.</param>
        /// <param name="category">A string specifying the translation category (domain) to use.</param>
        /// <param name="profanityAction">Specifies how profanity in the input text should be handled.</param>
        /// <param name="profanityMarker">Specifies how profanity in the output text should be marked.</param>
        /// <param name="includeAlignment">Indicates whether to include alignment information in the response.</param>
        /// <param name="includeSentenceLength">Indicates whether to include sentence length information in the response.</param>
        /// <param name="suggestedFrom">A suggested source language code for the translation service to use.</param>
        /// <param name="fromScript">The script of the source language text.</param>
        /// <param name="toScript">The script to use for the translated text.</param>
        /// <param name="allowFallback">Indicates whether to allow fallback to a related language if translation to the target language is not available.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of <see cref="TranslatedTextItem"/> objects representing the translated text.</returns>
        Task<IReadOnlyList<TranslatedTextItem>> TranslateAsync(
            IEnumerable<string> targetLanguages,
            IEnumerable<string> content,
            Guid clientTraceId = default,
            string? sourceLanguage = null,
            TextType? textType = null,
            string? category = null,
            ProfanityAction? profanityAction = null,
            ProfanityMarker? profanityMarker = null,
            bool? includeAlignment = null,
            bool? includeSentenceLength = null,
            string? suggestedFrom = null,
            string? fromScript = null,
            string? toScript = null,
            bool? allowFallback = null,
            CancellationToken cancellationToken = default);
    }
}
