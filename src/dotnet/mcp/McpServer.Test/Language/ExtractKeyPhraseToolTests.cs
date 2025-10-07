// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.AI.Language.MCP.Server.Clients.Language;
using Azure.AI.Language.MCP.Server.Language.Tools;
using Azure.AI.Language.Text;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;
using Assert = Xunit.Assert;

namespace Azure.AI.Language.MCP.Server.Tests.Language
{
    public partial class ExtractKeyPhraseToolTests
    {
        private readonly Mock<ILogger<ExtractKeyPhraseTool>> _loggerMock;
        private readonly Mock<ILanguageClient> _clientMock;
        private readonly ExtractKeyPhraseTool _tool;

        public ExtractKeyPhraseToolTests()
        {
            _loggerMock = new Mock<ILogger<ExtractKeyPhraseTool>>();
            _clientMock = new Mock<ILanguageClient>();
            _tool = new ExtractKeyPhraseTool(_loggerMock.Object, _clientMock.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenClientIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExtractKeyPhraseTool(_loggerMock.Object, null));
        }

        [Fact]
        public async Task ExtractKeyPhrasesFromText_ReturnsErrorMessage_OnException()
        {
            // Arrange
            _clientMock.Setup(c => c.AnalyzeTextAsync(
                It.IsAny<AnalyzeTextInput>(),
                It.IsAny<bool?>(),
                It.IsAny<CancellationToken>())).Throws(new Exception("Test error"));

            // Act
            var result = await _tool.ExtractKeyPhrasesFromText("test");

            // Assert
            Assert.Contains("\"isError\":true", result);
        }

        [Fact]
        public async Task ExtractKeyPhrasesFromText_ReturnsSerializedResponse_OnSuccess()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ExtractKeyPhraseTool>>();
            var clientMock = new Mock<ILanguageClient>();

            var serializedResult = @"{}";

            var analyzeResult = JsonConvert.DeserializeObject<AnalyzeTextKeyPhraseResult>(serializedResult);

             var response = new TestResponse<AnalyzeTextResult>(analyzeResult);

            clientMock
                .Setup(c => c.AnalyzeTextAsync(It.IsAny<AnalyzeTextInput>(), null, default))
                .ReturnsAsync(response);

            var tool = new ExtractKeyPhraseTool(loggerMock.Object, clientMock.Object);

            // Act
            var result = await tool.ExtractKeyPhrasesFromText("My name is John Doe");

            // Assert
            Assert.DoesNotContain("\"isError\":true", result);
            Assert.Contains("content", result);
        }
    }
}
