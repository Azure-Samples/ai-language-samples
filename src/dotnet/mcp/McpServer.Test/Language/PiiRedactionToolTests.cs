// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.AI.Language.MCP.Server.Clients.Language;
using Azure.AI.Language.MCP.Server.Clients.Language.DocumentAnalysis.Models;
using Azure.AI.Language.MCP.Server.Language.Tools;
using Azure.AI.Language.Text;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;
using Assert = Xunit.Assert;

namespace Azure.AI.Language.MCP.Server.Tests.Language
{
    public partial class PiiRedactionToolTests
    {
        private readonly Mock<ILogger<PiiRedactionTool>> _loggerMock;
        private readonly Mock<ILanguageClient> _clientMock;
        private readonly PiiRedactionTool _tool;

        public PiiRedactionToolTests()
        {
            _loggerMock = new Mock<ILogger<PiiRedactionTool>>();
            _clientMock = new Mock<ILanguageClient>();
            _tool = new PiiRedactionTool(_loggerMock.Object, _clientMock.Object);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenClientIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new PiiRedactionTool(_loggerMock.Object, null));
        }

        [Fact]
        public async Task RedactPiiFromText_ReturnsErrorMessage_OnException()
        {
            // Arrange
            _clientMock.Setup(c => c.AnalyzeTextAsync(
                It.IsAny<AnalyzeTextInput>(),
                It.IsAny<bool?>(),
                It.IsAny<CancellationToken>())).Throws(new Exception("Test error"));

            // Act
            string result = await _tool.RedactPiiFromText("test");

            // Assert
            Assert.Contains("Test error", result);
            Assert.Contains("\"isError\":true", result);
        }

        [Fact]
        public async Task RedactPiiFromDocument_ReturnsSerializedResponse_OnSuccess()
        {
            // Arrange
            Uri source = new Uri("https://test/source.docx");
            Uri target = new Uri("https://test/target/");
            AnalyzeDocumentsJobState jobState = new AnalyzeDocumentsJobState
            {
                Status = "succeeded",
                Tasks = new AnalyzeDocumentsJobResult(1, 1, 0, 0)
                {
                    Items = new List<AnalyzeDocumentsJobTaskResult>
                    {
                        new AnalyzeDocumentsJobTaskResult()
                        {
                            Kind = "DocumentPIILroResults",
                            Results = new AnalyzeDocumentsResult()
                            {
                                Documents =
                                [
                                    new DocumentResult()
                                    {
                                        Source =  new DocumentLocation(source.ToString()),
                                        Targets = [new(target.ToString())],
                                    }
                                ]
                            }

                        }
                    }
                }

            };
            _clientMock.Setup(c => c.AnalyzeDocumentAsync(It.IsAny<DocumentPiiEntityRecognitionInput>(), default))
                .ReturnsAsync(jobState);

            // Act
            string result = await _tool.RedactPiiFromDocument(source, target);

            // Assert
            Assert.Contains(source.ToString(), result);
        }

        [Fact]
        public async Task RedactPiiFromText_ReturnsSerializedResponse_OnSuccess()
        {
            // Arrange
            Mock<ILogger<PiiRedactionTool>> loggerMock = new Mock<ILogger<PiiRedactionTool>>();
            Mock<ILanguageClient> clientMock = new Mock<ILanguageClient>();

            string serializedResult = @"{}";

            AnalyzeTextPiiResult? analyzeResult = JsonConvert.DeserializeObject<AnalyzeTextPiiResult>(serializedResult);

            TestResponse<AnalyzeTextResult> response = new TestResponse<AnalyzeTextResult>(analyzeResult);

            clientMock
                .Setup(c => c.AnalyzeTextAsync(It.IsAny<AnalyzeTextInput>(), null, default))
                .ReturnsAsync(response);

            PiiRedactionTool tool = new PiiRedactionTool(loggerMock.Object, clientMock.Object);

            // Act
            string result = await tool.RedactPiiFromText("My name is John Doe");

            // Assert
            Assert.DoesNotContain("\"isError\":true", result);
            Assert.Contains("content", result);
        }

        [Fact]
        public async Task RedactPiiFromDocument_ReturnsErrorResponse_OnException()
        {
            // Arrange
            _clientMock.Setup(c => c.AnalyzeDocumentAsync(It.IsAny<DocumentPiiEntityRecognitionInput>(), default))
                .ThrowsAsync(new Exception("Document error"));

            // Act
            string result = await _tool.RedactPiiFromDocument(new Uri("https://test/source.docx"), new Uri("https://test/target/"));

            // Assert
            Assert.Contains("Document error", result);
            Assert.Contains("\"isError\":true", result);
        }
    }
}
