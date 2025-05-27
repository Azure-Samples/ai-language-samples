// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;
using Azure.AI.Language.MCP.Server.Clients.Translator;
using Azure.AI.Language.MCP.Server.Response;
using Azure.AI.Translation.Text;
using LanguageAgentTools.Clients.Utils;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace Azure.AI.Language.MCP.Server.Tools
{
    /// <summary>
    /// This class provides methods for translating text from one language to another using the Azure.AI.Language Translation service.
    /// </summary>
    [McpServerToolType]
    internal sealed class TranslatorTool
    {
        private const string TranslatorToolDescriptions = "Translate text from one language to another. It requires the text to be translated and the list of languages to be translated to. Optionally it accepts the source language. It returns a json array containing the translations.";
        private readonly ITranslatorClient client;
        private readonly ILogger logger;

        public TranslatorTool(ILogger<TranslatorTool> logger, ITranslatorClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            this.logger = logger;
            this.client = client;
        }

        [McpServerTool(Title = "Text Translation Tool"), Description(TranslatorToolDescriptions)]
        public async Task<string> Translate(
            [Description("The text to be translated")] string message,
            [Description("The list of language in 2 letter iso format, the input message should be translated to")] IList<string> targetLanguages,
            [Description("(Optional) The language of the input text. Default value is null, where the tool will perform auto language detection.")] string sourceLanguage = default,
            ILogger logger = default)
        {
            try
            {
                logger.LogInformation($"Source language: {sourceLanguage} and target languages: {string.Join(',', targetLanguages)}");

                IReadOnlyList<TranslatedTextItem> response = await client.TranslateAsync(targetLanguages, new List<string> { message }, sourceLanguage: sourceLanguage);
                var serializedResponse = response.ToTranslationResult();

                return new McpResponse(serializedResponse).Serialize();

            }
            catch (Exception ex)
            {
                logger.LogInformation(ex, $"Error in TranslatorTool: {ex.Message}");
                return new McpResponse(ex.Message, true).Serialize();
            }
        }
    }
}