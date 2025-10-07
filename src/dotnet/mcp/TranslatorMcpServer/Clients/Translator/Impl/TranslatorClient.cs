// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.AI.Translation.Text;

namespace Azure.AI.Language.MCP.Server.Translator.Clients.Translator.Impl
{
    /// <summary>
    /// Implementation of the <see cref="ITranslatorClient"/> interface that uses the Azure AI Translation service to translate text.
    /// </summary>
    internal sealed class TranslatorClient : ITranslatorClient
    {
        private readonly TextTranslationClient client;

        public TranslatorClient(TextTranslationClient client)
        {
            ArgumentNullException.ThrowIfNull(client);
            this.client = client;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<TranslatedTextItem>> TranslateAsync(
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
            CancellationToken cancellationToken = default)
        {
            var response = await this.client.TranslateAsync(
                targetLanguages,
                content,
                clientTraceId,
                sourceLanguage,
                textType,
                category,
                profanityAction,
                profanityMarker,
                includeAlignment,
                includeSentenceLength,
                suggestedFrom,
                fromScript,
                toScript,
                allowFallback,
                cancellationToken);

            return response.Value;
        }
    }
}
