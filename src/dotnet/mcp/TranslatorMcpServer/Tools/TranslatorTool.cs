// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;
using Azure.AI.Language.MCP.Common.Logger;
using Azure.AI.Language.MCP.Common.Response;
using Azure.AI.Language.MCP.Server.Translator.Clients.Translator;
using Azure.AI.Translation.Text;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Azure.AI.Language.MCP.Server.Translator.Tools
{
    /// <summary>
    /// This class provides methods for translating text from one language to another using the Azure.AI.Language Translation service.
    /// </summary>
    [McpServerToolType]
    public sealed class TranslatorTool
    {
        private const string TranslatorToolDescriptions = "This tool translates text from one language to another. " +
            "It requires the text to be translated and the list of languages to be translated to. " +
            "Optionally it accepts the source language. " +
            "It returns a json array containing the translations.";

        private static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };

        private readonly ITranslatorClient client;
        private readonly ILogger logger;

        public TranslatorTool(ILogger<TranslatorTool> logger, ITranslatorClient client)
        {
            ArgumentNullException.ThrowIfNull(client);
            this.logger = logger ?? new ConsoleLogger<TranslatorTool>();
            this.client = client;
        }

        [McpServerTool(Title = "Text Translation Tool")]
        [Description(TranslatorToolDescriptions)]
        public async Task<string> Translate(
            [Description("The text to be translated")] string message,
            [Description("The list of language in 2 letter iso format, the input message should be translated to")] IList<string> targetLanguages,
            [Description("(Optional) The language of the input text. Default value is null, where the tool will perform auto language detection.")] string sourceLanguage = default)
        {
            try
            {
                IReadOnlyList<TranslatedTextItem> response = await this.client.TranslateAsync(
                    targetLanguages,
                    [message],
                    sourceLanguage: sourceLanguage);

                string serializedResponse = JsonConvert.SerializeObject(response.Select(item => item.Translations), DefaultJsonSerializerSettings);
                return new McpResponse(serializedResponse).Serialize();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error in TranslatorTool: {ErrorMessage}", ex.Message);
                return new McpResponse(ex.Message, true).Serialize();
            }
        }
    }
}