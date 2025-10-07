// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.AI.Language.MCP.Server.Clients.Language.Models;
using Azure.AI.Language.MCP.Server.Clients.Language.Text;
using Azure.AI.Language.MCP.Server.Enums.Healthcare;
using Azure.AI.Language.MCP.Server.Enums.Ner;
using Azure.AI.Language.MCP.Server.Enums.Pii;
using Azure.AI.Language.Text;

namespace Azure.AI.Language.MCP.Server.Clients.Utils
{
    /// <summary>
    /// Helper class for creating input payloads and action contents for Analyze Text operations.
    /// </summary>
    public static class AnalyzeTextClientHelper
    {
        private const string DefaultInputId = "1";

        /// <summary>
        /// Creates an input for PII entities recognition.
        /// </summary>
        /// <param name="message">The text message to analyze.</param>
        /// <param name="language">The language of the text.</param>
        /// <param name="modelVersion">The model version to use.</param>
        /// <param name="piiCategories">List of PII categories to include.</param>
        /// <param name="excludePiiCategories">List of PII categories to exclude.</param>
        /// <param name="redactionPolicy">The redaction policy to apply.</param>
        /// <param name="redactionCharacter">The character to use for redaction.</param>
        /// <returns>An <see cref="AnalyzeTextInput"/> for PII entities recognition.</returns>
        public static AnalyzeTextInput CreateTextPiiEntitiesRecognitionInput(
            string message,
            string language,
            string modelVersion,
            IList<PiiCategoryEnum> piiCategories,
            IList<PiiCategoryEnum> excludePiiCategories,
            RedactionPolicyEnum redactionPolicy,
            char redactionCharacter)
        {
            MultiLanguageTextInput input = CreateTextAnalysisInputPayload(message, language);

            PiiActionContent piiContent = CreatePiiActionContent(modelVersion, piiCategories, excludePiiCategories, redactionPolicy, redactionCharacter);

            return new TextPiiEntitiesRecognitionInput()
            {
                TextInput = input,
                ActionContent = piiContent,
            };
        }

        /// <summary>
        /// Creates an input for entity recognition.
        /// </summary>
        /// <param name="message">The text message to analyze.</param>
        /// <param name="language">The language of the text.</param>
        /// <param name="modelVersion">The model version to use.</param>
        /// <param name="inclusionList">List of NER types to include.</param>
        /// <param name="exclusionList">List of NER types to exclude.</param>
        /// <param name="overlapPolicy">The overlap policy to apply.</param>
        /// <returns>An <see cref="AnalyzeTextInput"/> for entity recognition.</returns>
        public static AnalyzeTextInput CreateTextEntityRecognitionInput(
            string message,
            string language,
            string modelVersion,
            IList<NerTypeEnum> inclusionList,
            IList<NerTypeEnum> exclusionList,
            OverlapPolicyEnum overlapPolicy)
        {
            MultiLanguageTextInput input = CreateTextAnalysisInputPayload(message, language);

            EntitiesActionContent actionContent = CreateNamedEntityActionContent(modelVersion, inclusionList, exclusionList, overlapPolicy);

            return new TextEntityRecognitionInput()
            {
                TextInput = input,
                ActionContent = actionContent,
            };
        }

        /// <summary>
        /// Creates an input for language detection.
        /// </summary>
        /// <param name="message">The text message to analyze.</param>
        /// <param name="modelVersion">The model version to use.</param>
        /// <param name="countryHint">The country hint for language detection.</param>
        /// <returns>An <see cref="AnalyzeTextInput"/> for language detection.</returns>
        public static AnalyzeTextInput CreateLanguageDetectionInput(
            string message,
            string modelVersion,
            string countryHint)
        {
            LanguageDetectionTextInput input = CreateLanguageDetectionInputPayload(message, countryHint);

            LanguageDetectionActionContent actionContent = CreateLanguageDetectionActionContent(modelVersion);

            return new TextLanguageDetectionInput()
            {
                TextInput = input,
                ActionContent = actionContent,
            };
        }

        /// <summary>
        /// Creates an input for extract key phrases.
        /// </summary>
        /// <param name="message">The text message to analyze.</param>
        /// <param name="language">The language of the text.</param>
        /// <param name="modelVersion">The model version to use.</param>
        /// <returns>An <see cref="AnalyzeTextInput"/> for entity recognition.</returns>
        public static AnalyzeTextInput CreateExtractKeyPhraseInput(
            string message,
            string language,
            string modelVersion)
        {
            MultiLanguageTextInput input = CreateTextAnalysisInputPayload(message, language);

            KeyPhraseActionContent actionContent = CreateExtractKeyPhrasesActionContent(modelVersion);

            return new TextKeyPhraseExtractionInput()
            {
                TextInput = input,
                ActionContent = actionContent,
            };
        }

        /// <summary>
        /// Creates an input for sentiment analysis.
        /// </summary>
        /// <param name="message">The text message to analyze.</param>
        /// <param name="language">The language of the text.</param>
        /// <param name="modelVersion">The model version to use.</param>
        /// <param name="opinionMining">Whether to enable opinion mining.</param>
        /// <returns>An <see cref="AnalyzeTextInput"/> for entity recognition.</returns>
        public static AnalyzeTextInput CreateSentimentAnalysisInput(
            string message,
            string language,
            string modelVersion,
            bool opinionMining)
        {
            MultiLanguageTextInput input = CreateTextAnalysisInputPayload(message, language);

            SentimentAnalysisActionContent actionContent = CreateSentimentAnalysisActionContent(modelVersion, opinionMining);

            return new TextSentimentAnalysisInput()
            {
                TextInput = input,
                ActionContent = actionContent,
            };
        }

        /// <summary>
        /// Creates an input for text summarization operations.
        /// </summary>
        /// <param name="message">The text message to summarize.</param>
        /// <param name="language">The language of the text.</param>
        /// <param name="modelVersion">The model version to use.</param>
        /// <param name="summarizationType">The type of summarization ("extractive" or "abstractive").</param>
        /// <param name="summaryLengthBucket">The summary length bucket for abstractive summarization.</param>
        /// <param name="sentenceCount">The number of sentences for extractive summarization.</param>
        /// <returns>An <see cref="AnalyzeTextOperationsInput"/> for text summarization.</returns>
        public static AnalyzeTextOperationsInput CreateAnalyzeTextSummarizationOperationsInput(
            string message,
            string language,
            string modelVersion,
            string summarizationType,
            SummaryLengthBucket summaryLengthBucket,
            int? sentenceCount)
        {
            MultiLanguageTextInput input = CreateTextAnalysisInputPayload(message, language);

            if (summarizationType == "extractive")
            {
                ExtractiveSummarizationOperationAction extractionOperationAction = CreateExtractiveSummarizationOperationAction(modelVersion, sentenceCount);

                return new AnalyzeTextOperationsInput(input, extractionOperationAction);
            }

            AbstractiveSummarizationOperationAction abstractiveOperationAction = CreateAbstractiveSummarizationOperationAction(modelVersion, summaryLengthBucket);

            return new AnalyzeTextOperationsInput(input, abstractiveOperationAction);
        }

        /// <summary>
        /// Creates an input for healthcare text analysis operations.
        /// </summary>
        /// <param name="message">The healthcare text message to analyze.</param>
        /// <param name="language">The language of the text.</param>
        /// <param name="modelVersion">The model version to use.</param>
        /// <param name="healthcareDocumentType">The type of healthcare document.</param>
        /// <param name="fhirVersion">The FHIR version to use.</param>
        /// <returns>An <see cref="AnalyzeTextOperationsInput"/> for healthcare text analysis.</returns>
        public static AnalyzeTextOperationsInput CreateHealthcareAnalyzeTextOperationsInput(
            string message,
            string language,
            string modelVersion,
            HealthcareDocumentTypeEnum healthcareDocumentType,
            string fhirVersion)
        {
            MultiLanguageTextInput input = CreateTextAnalysisInputPayload(message, language);

            HealthcareOperationAction operationAction = CreateHealthcareOperationAction(modelVersion, healthcareDocumentType, fhirVersion);

            return new AnalyzeTextOperationsInput(input, operationAction);
        }

        internal static HealthcareOperationAction CreateHealthcareOperationAction(string modelVersion, HealthcareDocumentTypeEnum healthcareDocumentType, string fhirVersion)
        {
            var actionContent = new HealthcareActionContent()
            {
                DocumentType = healthcareDocumentType.ToString(),
                FhirVersion = fhirVersion,
                ModelVersion = modelVersion,
            };

            var operationAction = new HealthcareOperationAction()
            {
                ActionContent = actionContent,
            };
            return operationAction;
        }

        internal static AbstractiveSummarizationOperationAction CreateAbstractiveSummarizationOperationAction(
            string modelVersion,
            SummaryLengthBucket summaryLengthBucket)
        {
            var actionContent = new AbstractiveSummarizationActionContent()
            {
                ModelVersion = modelVersion,
                SummaryLength = summaryLengthBucket,
            };

            var operationAction = new AbstractiveSummarizationOperationAction()
            {
                ActionContent = actionContent,
            };
            return operationAction;
        }

        internal static ExtractiveSummarizationOperationAction CreateExtractiveSummarizationOperationAction(
            string modelVersion,
            int? sentenceCount)
        {
            var actionContent = new ExtractiveSummarizationActionContent()
            {
                ModelVersion = modelVersion,
                SentenceCount = sentenceCount,
            };

            var operationAction = new ExtractiveSummarizationOperationAction()
            {
                ActionContent = actionContent,
            };
            return operationAction;
        }

        internal static MultiLanguageTextInput CreateTextAnalysisInputPayload(string message, string language)
        {
            var input = new MultiLanguageTextInput();
            input.MultiLanguageInputs.Add(new MultiLanguageInput(DefaultInputId, message)
            {
                Language = language,
            });
            return input;
        }

        internal static LanguageDetectionTextInput CreateLanguageDetectionInputPayload(string message, string countryHint)
        {
            var input = new LanguageDetectionTextInput();
            input.LanguageInputs.Add(new LanguageInput(DefaultInputId, message)
            {
                CountryHint = countryHint,
            });
            return input;
        }

        internal static PiiActionContent CreatePiiActionContent(
            string modelVersion,
            IList<PiiCategoryEnum> piiCategories,
            IList<PiiCategoryEnum> excludePiiCategories,
            RedactionPolicyEnum redactionPolicy,
            char redactionCharacter)
        {
            var piiContent = new PiiActionContent()
            {
                RedactionPolicy = GetRedactionPolicy(redactionPolicy, redactionCharacter),
                ModelVersion = modelVersion,
            };

            if (piiCategories?.Any() ?? false)
            {
                foreach (PiiCategoryEnum category in piiCategories)
                {
                    piiContent.PiiCategories.Add(new PiiCategory(category.ToString()));
                }
            }

            if (excludePiiCategories?.Any() ?? false)
            {
                foreach (PiiCategoryEnum category in excludePiiCategories)
                {
                    piiContent.ExcludePiiCategories.Add(new PiiCategoriesExclude(category.ToString()));
                }
            }

            return piiContent;
        }

        internal static LanguageDetectionActionContent CreateLanguageDetectionActionContent(string modelVersion)
        {
            var languageContent = new LanguageDetectionActionContent()
            {
                ModelVersion = modelVersion,
            };

            return languageContent;
        }

        internal static SentimentAnalysisActionContent CreateSentimentAnalysisActionContent(string modelVersion, bool opinionMining)
        {
            var sentimentContent = new SentimentAnalysisActionContent()
            {
                ModelVersion = modelVersion,
                OpinionMining = opinionMining,
            };

            return sentimentContent;
        }

        internal static KeyPhraseActionContent CreateExtractKeyPhrasesActionContent(string modelVersion)
        {
            var keyPhraseContent = new KeyPhraseActionContent()
            {
                ModelVersion = modelVersion,
            };

            return keyPhraseContent;
        }

        internal static EntitiesActionContent CreateNamedEntityActionContent(
            string modelVersion,
            IList<NerTypeEnum> inclusionList,
            IList<NerTypeEnum> exclusionList,
            OverlapPolicyEnum overlapPolicy)
        {
            var actionContent = new EntitiesActionContent()
            {
                ModelVersion = modelVersion,
            };

            if (inclusionList?.Any() ?? false)
            {
                foreach (var entityTypes in inclusionList)
                {
                    actionContent.Inclusions.Add(new EntityCategory(entityTypes.ToString()));
                }
            }

            if (exclusionList?.Any() ?? false)
            {
                foreach (var category in exclusionList)
                {
                    actionContent.Exclusions.Add(new EntityCategory(category.ToString()));
                }
            }

            if (overlapPolicy != default)
            {
                actionContent.OverlapPolicy = overlapPolicy.ToOverlapPolicy();
            }

            return actionContent;
        }

        internal static BaseRedactionPolicy GetRedactionPolicy(RedactionPolicyEnum redactionPolicy, char redactionCharacter)
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

        internal static RedactionCharacter? ToRedactionCharacter(char redactionCharacter)
        {
            return redactionCharacter switch
            {
                '!' => (RedactionCharacter?)RedactionCharacter.ExclamationPoint,
                '#' => (RedactionCharacter?)RedactionCharacter.NumberSign,
                '$' => (RedactionCharacter?)RedactionCharacter.Dollar,
                '%' => (RedactionCharacter?)RedactionCharacter.PerCent,
                '&' => (RedactionCharacter?)RedactionCharacter.Ampersand,
                '*' => (RedactionCharacter?)RedactionCharacter.Asterisk,
                '+' => (RedactionCharacter?)RedactionCharacter.Plus,
                '-' => (RedactionCharacter?)RedactionCharacter.Minus,
                '=' => (RedactionCharacter?)RedactionCharacter.EqualsValue,
                '?' => (RedactionCharacter?)RedactionCharacter.QuestionMark,
                '@' => (RedactionCharacter?)RedactionCharacter.AtSign,
                '^' => (RedactionCharacter?)RedactionCharacter.Caret,
                '_' => (RedactionCharacter?)RedactionCharacter.Underscore,
                '~' => (RedactionCharacter?)RedactionCharacter.Tilde,
                _ => (RedactionCharacter?)RedactionCharacter.Asterisk,
            };
        }
    }
}
