// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.AI.Language.MCP.Server.Clients.Translator;
using Azure.AI.Language.MCP.Server.Tools;
using Azure.AI.Translation.Text;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Azure.AI.Language.MCP.Server.Tests.Tools
{
    public class TranslatorToolTests
    {
        private readonly Mock<ITranslatorClient> _mockClient;
        private readonly Mock<ILogger<TranslatorTool>> _mockLogger;
        private readonly TranslatorTool _tool;

        public TranslatorToolTests()
        {
            _mockClient = new Mock<ITranslatorClient>();
            _mockLogger = new Mock<ILogger<TranslatorTool>>();
            _tool = new TranslatorTool(_mockLogger.Object, _mockClient.Object);
        }

        [Fact]
        public async Task Translate_ReturnsSerializedResponse_OnSuccess()
        {
            // Arrange
            var message = "Hello";
            var targetLanguages = new List<string> { "fr" };
            var sourceLanguage = "en";
            var translatedItems = new List<TranslatedTextItem>
            {
            };

            _mockClient.Setup(c => c.TranslateAsync(
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<TextType?>(),
                It.IsAny<string>(),
                It.IsAny<ProfanityAction?>(),
                It.IsAny<ProfanityMarker?>(),
                It.IsAny<bool?>(),
                It.IsAny<bool?>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool?>(),
                It.IsAny<System.Threading.CancellationToken>()
            )).ReturnsAsync(translatedItems);

            // Act
            var result = await _tool.Translate(message, targetLanguages, sourceLanguage, _mockLogger.Object);

            // Assert
            Assert.DoesNotContain("\"isError\":true", result);
        }

        [Fact]
        public async Task Translate_ReturnsErrorResponse_OnException()
        {
            // Arrange
            var message = "Hello";
            var targetLanguages = new List<string> { "fr" };
            var sourceLanguage = "en";
            _mockClient.Setup(c => c.TranslateAsync(
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<TextType?>(),
                It.IsAny<string>(),
                It.IsAny<ProfanityAction?>(),
                It.IsAny<ProfanityMarker?>(),
                It.IsAny<bool?>(),
                It.IsAny<bool?>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool?>(),
                It.IsAny<System.Threading.CancellationToken>()
            )).ThrowsAsync(new Exception("Translation failed"));

            // Act
            var result = await _tool.Translate(message, targetLanguages, sourceLanguage, _mockLogger.Object);

            // Assert
            Assert.Contains("Translation failed", result);
            Assert.Contains("\"isError\":true", result);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenClientIsNull()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new TranslatorTool(_mockLogger.Object, null));
        }
    }
}
