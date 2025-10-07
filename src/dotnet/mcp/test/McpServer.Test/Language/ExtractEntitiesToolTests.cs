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
    public partial class ExtractEntitiesToolTests
    {
        private readonly Mock<ILogger<ExtractEntitiesTool>> _loggerMock;
        private readonly Mock<ILanguageClient> _clientMock;
        private readonly ExtractEntitiesTool _tool;

        public ExtractEntitiesToolTests()
        {
            _loggerMock = new Mock<ILogger<ExtractEntitiesTool>>();
            _clientMock = new Mock<ILanguageClient>();
            _tool = new ExtractEntitiesTool(_loggerMock.Object, _clientMock.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenClientIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ExtractEntitiesTool(_loggerMock.Object, null));
        }

        [Fact]
        public async Task ExtractEntitiesFromText_ReturnsErrorMessage_OnException()
        {
            // Arrange
            _clientMock.Setup(c => c.AnalyzeTextAsync(
                It.IsAny<AnalyzeTextInput>(),
                It.IsAny<bool?>(),
                It.IsAny<CancellationToken>())).Throws(new Exception("Test error"));

            // Act
            var result = await _tool.ExtractNamedEntitiesFromText("test");

            // Assert
            Assert.Contains("Test error", result);
            Assert.Contains("\"isError\":true", result);
        }

        [Fact]
        public async Task ExtractEntitiesFromText_ReturnsSerializedResponse_OnSuccess()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ExtractEntitiesTool>>();
            var clientMock = new Mock<ILanguageClient>();

            var serializedResult = @"{}";

            var analyzeResult = JsonConvert.DeserializeObject<AnalyzeTextEntitiesResult>(serializedResult);

             var response = new TestResponse<AnalyzeTextResult>(analyzeResult);

            clientMock
                .Setup(c => c.AnalyzeTextAsync(It.IsAny<AnalyzeTextInput>(), null, default))
                .ReturnsAsync(response);

            var tool = new ExtractEntitiesTool(loggerMock.Object, clientMock.Object);

            // Act
            var result = await tool.ExtractNamedEntitiesFromText("My name is John Doe");

            // Assert
            Assert.DoesNotContain("\"isError\":true", result);
            Assert.Contains("content", result);
        }
    }
}
