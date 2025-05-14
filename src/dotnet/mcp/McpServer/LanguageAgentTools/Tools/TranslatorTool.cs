// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel;
using Azure;
using Azure.AI.Translation.Text;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

using Newtonsoft.Json;

namespace LanguageAgentTools
{
    [McpServerToolType]
    internal class TranslatorTool
    {
        [McpServerTool, Description("Translate text from one language to another")]
        public static string Translate(
            TextTranslationClient client,
            [Description("The text to be translated")] string message,
            [Description("The list of language in 2 letter iso format, the input message should be translated to")] IList<string> targetLanguages,
            [Description("(Optional) The language of the input text. Default value is null, where the tool will perform auto language detection.")] string sourceLanguage = default,
            ILogger logger = default)
        {
            try
            {
                logger.LogInformation($"Source language: {sourceLanguage} and target languages: {string.Join(',', targetLanguages)}");

                Response<IReadOnlyList<TranslatedTextItem>> response = client.Translate(targetLanguages, new List<string> { message }, sourceLanguage: sourceLanguage);
                return JsonConvert.SerializeObject(response.Value);  
            }
            catch (Exception ex)
            {
                logger.LogInformation(ex, $"Error in TranslatorTool: {ex.Message}");
                return $"Error in TranslatorTool: {ex.Message}";
            }
        }
    }
}