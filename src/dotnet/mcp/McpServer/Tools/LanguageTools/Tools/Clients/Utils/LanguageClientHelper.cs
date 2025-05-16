// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.AI.Language.Text;
using Newtonsoft.Json;
using static Azure.AI.Language.Text.TextAnalysisClientOptions;
using LanguageAgentTools.Tools.Contracts;
using LanguageAgentTools.Tools.Clients.DocumentAnalysis.Models;

namespace LanguageAgentTools.Tools.Clients.Utils
{
    /// <summary>
    /// Provides helper methods for creating text analysis task inputs and transforming results.
    /// </summary>
    internal static class LanguageClientHelper
    {
        private const string DefaultInputId = "1";

        /// <summary>
        /// Creates an input for text analysis tasks.
        /// </summary>
        /// <param name="message">The list of text inputs.</param>
        /// <returns>An instance of <see cref="AnalyzeTextInput"/>.</returns>
        public static AnalyzeTextInput CreateTextAnalysisTaskInput(
            string message,
            IList<string> piiCategories,
            IList<string> excludePiiCategories, 
            RedactionPolicyEnum redactionPolicy,
            char redactionCharacter,
            string language,
            string modelVersion)
        {
            var input = new MultiLanguageTextInput();
            input.MultiLanguageInputs.Add(new MultiLanguageInput(DefaultInputId, message)
            {
                Language = language
            });

            var piiContent = new PiiActionContent()
            {
                RedactionPolicy = GetRedactionPolicy(redactionPolicy, redactionCharacter),
                ModelVersion = modelVersion
            };

            if (piiCategories?.Any() ?? false)
            {
                foreach (var category in piiCategories)
                {
                    if (category.Equals("All", StringComparison.CurrentCultureIgnoreCase))
                    {
                        piiContent.PiiCategories.Add(PiiCategory.All);
                        break;
                    }
                    piiContent.PiiCategories.Add(new PiiCategory(category));
                }
            }

            if (excludePiiCategories?.Any() ?? false)
            {
                foreach (var category in excludePiiCategories)
                {
                    piiContent.ExcludePiiCategories.Add(new PiiCategoriesExclude(category.ToString()));
                }
            }

            return new TextPiiEntitiesRecognitionInput()
            {
                TextInput = input,
                ActionContent = piiContent,
            };
        }

        public static DocumentPiiEntityRecognitionInput CreateDocumentAnalysisTaskInput(
            Uri source,
            Uri target,
            IList<string> piiCategories,
            IList<string> excludePiiCategories,
            string redactionPolicy,
            char redactionCharacter,
            string language, 
            string modelVersion)
        {
            var input = new AnalyzeDocumentsInput();

            var sourceLocation = new DocumentLocation(source.ToString());
            var targetLocation = new DocumentLocation(target.ToString());

            input.Documents.Add(new DocumentInput(DefaultInputId, sourceLocation, targetLocation, language));


            var tasks = new AnalyzeDocumentsTask[]
            {
                new AnalyzeDocumentsTask()
                {
                    Kind = AnalyzeDocumentsTaskKind.PiiEntityRecognition,
                    Parameters = new DocumentPiiEntityRecognitionTaskParameters(piiCategories, excludePiiCategories, new RedactionPolicy(redactionPolicy, redactionCharacter), modelVersion),
                    
                }
            };

            return new DocumentPiiEntityRecognitionInput(input, tasks);
        }

        public static RedactionPolicyKind ToRedactionPolicyKind(this RedactionPolicyEnum redactionPolicyEnum)
        {
            return redactionPolicyEnum switch
            {
                RedactionPolicyEnum.EntityMask => RedactionPolicyKind.EntityMask,
                RedactionPolicyEnum.NoMask => RedactionPolicyKind.NoMask,
                _ => RedactionPolicyKind.CharacterMask,
            };
        }

        /// <summary>
        /// Gets the service version for text analytics based on the API version string.
        /// </summary>
        /// <param name="apiVersion">The API version string.</param>
        /// <returns>The corresponding <see cref="ServiceVersion"/>.</returns>
        public static ServiceVersion GetTextAnalyticsServiceVersion(string apiVersion)
        {
            if (!string.IsNullOrEmpty(apiVersion))
            {
                var serviceVersion = $"V{apiVersion.Replace('-', '_')}";
                if (Enum.TryParse<ServiceVersion>(serviceVersion, out var version))
                {
                    return version;
                }
            }
            return Enum.GetValues(typeof(ServiceVersion)).Cast<ServiceVersion>().Last();
        }

        /// <summary>
        /// Converts the API response to a concise format for MCP response.
        /// </summary>
        /// <param name="response">The SDK response.</param>
        /// <returns>The transformed <see cref="LanguageTextResult"/>.</returns>
        public static string ToPiiEntityRecognitionResult(this AnalyzeTextResult response)
        {
            if (response is AnalyzeTextPiiResult piiResult)
            {
                if (piiResult.Results.Errors.Any())
                {
                    return JsonConvert.SerializeObject(piiResult.Results.Errors);
                }
                return JsonConvert.SerializeObject(piiResult.Results.Documents);

            }
            return "Not supported feature.";
        }

        /// <summary>
        /// Converts the API response to a concise format for MCP response.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string ToDocumentPiiEntityRecognitionResult(this AnalyzeDocumentsJobState response)
        {
            if(response.Error != null)
            {
                return JsonConvert.SerializeObject(response.Error);
            }

            if (response.Errors.Any())
            {
                return JsonConvert.SerializeObject(response.Errors);
            }

            var errorsInResponse = response.Tasks.Items.Where(task => task.Results.Errors.Any());

            if (errorsInResponse.Any())
            {
                return JsonConvert.SerializeObject(errorsInResponse.SelectMany(task => task.Results.Errors));
            }

            return JsonConvert.SerializeObject(response.Tasks.Items.SelectMany(task => task.Results.Documents));
        }

        private static BaseRedactionPolicy GetRedactionPolicy(RedactionPolicyEnum redactionPolicy, char redactionCharacter)
        {
            return redactionPolicy switch
            {
                RedactionPolicyEnum.EntityMask => new EntityMaskPolicyType(),
                RedactionPolicyEnum.NoMask => new NoMaskPolicyType(),
                _ => new CharacterMaskPolicyType()
                {
                    RedactionCharacter = ToRedactionCharacter(redactionCharacter),
                },
            };
        }

        private static RedactionCharacter? ToRedactionCharacter(char redactionCharacter)
        {
            switch (redactionCharacter)
            {
                case '!':
                    return RedactionCharacter.ExclamationPoint;
                case '#':
                    return RedactionCharacter.NumberSign;
                case '$':
                    return RedactionCharacter.Dollar;
                case '%':
                    return RedactionCharacter.PerCent;
                case '&':
                    return RedactionCharacter.Ampersand;
                case '*':
                    return RedactionCharacter.Asterisk;
                case '+':
                    return RedactionCharacter.Plus;
                case '-':
                    return RedactionCharacter.Minus;
                case '=':
                    return RedactionCharacter.EqualsValue;
                case '?':
                    return RedactionCharacter.QuestionMark;
                case '@':
                    return RedactionCharacter.AtSign;
                case '^':
                    return RedactionCharacter.Caret;
                case '_':
                    return RedactionCharacter.Underscore;
                case '~':
                    return RedactionCharacter.Tilde;
                default:
                    return RedactionCharacter.Asterisk;
            }
        }
    }
}
