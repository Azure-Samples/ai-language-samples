// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.AI.Translation.Text;

namespace Azure.AI.Language.MCP.Server.Clients.Translator
{
    /// <summary>
    /// Interface for a client that interacts with the Azure AI Translation service to translate text.
    /// </summary>
    public interface ITranslatorClient
    {
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
