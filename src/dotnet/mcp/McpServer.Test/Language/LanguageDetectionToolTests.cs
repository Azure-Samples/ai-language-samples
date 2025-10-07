using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.AI.Language.MCP.Server.Clients.Language;
using Azure.AI.Language.MCP.Server.Language.Tools;
using Azure.AI.Language.Text;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Azure.AI.Language.MCP.Server.Tests.Language
{
    public class LanguageDetectionToolTests
    {
        private readonly Mock<ILogger<LanguageDetectionTool>> _loggerMock;
        private readonly Mock<ILanguageClient> _clientMock;
        private readonly LanguageDetectionTool _tool;

        public LanguageDetectionToolTests()
        {
            _loggerMock = new Mock<ILogger<LanguageDetectionTool>>();
            _clientMock = new Mock<ILanguageClient>();
            _tool = new LanguageDetectionTool(_loggerMock.Object, _clientMock.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenClientIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new LanguageDetectionTool(_loggerMock.Object, null));
        }

        [Fact]
        public async Task DetectLanguageFromText_ReturnsErrorMessage_OnException()
        {
            // Arrange
            _clientMock.Setup(c => c.AnalyzeTextAsync(
                It.IsAny<AnalyzeTextInput>(),
                It.IsAny<bool?>(),
                It.IsAny<CancellationToken>())).Throws(new Exception("Test error"));

            // Act
            var result = await _tool.DetectLanguageFromText("test");

            // Assert
            Assert.Contains("Test error", result);
            Assert.Contains("\"isError\":true", result);
        }

        [Fact]
        public async Task DetectLanguageFromText_ReturnsSerializedResponse_OnSuccess()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<LanguageDetectionTool>>();
            var clientMock = new Mock<ILanguageClient>();

            var serializedResult = @"{}";

            var analyzeResult = JsonConvert.DeserializeObject<AnalyzeTextLanguageDetectionResult>(serializedResult);

            var response = new TestResponse<AnalyzeTextResult>(analyzeResult);

            clientMock
                .Setup(c => c.AnalyzeTextAsync(It.IsAny<AnalyzeTextInput>(), null, default))
                .ReturnsAsync(response);

            var tool = new LanguageDetectionTool(loggerMock.Object, clientMock.Object);

            // Act
            var result = await tool.DetectLanguageFromText("My name is John Doe");

            // Assert
            Assert.DoesNotContain("\"isError\":true", result);
            Assert.Contains("content", result);
        }
    }
}
