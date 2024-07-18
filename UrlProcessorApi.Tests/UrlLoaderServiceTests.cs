using Moq;
using Moq.Protected;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;

namespace UrlProcessorApi.Tests.Services
{
    public class UrlLoaderServiceTests
    {
        [Fact]
        public async Task ProcessUrls_ReturnsContent_ForValidUrls()
        {
            // Sample complex HTML input
            var HtmlInput = @"
            <!DOCTYPE html>
            <html>
            <head>
                <title>Test Page</title>
                <style>
                    body { font-family: Arial, sans-serif; }
                    .content { color: blue; }
                    .hidden { display: none; }
                </style>
                <script>
                    console.log('This is a test script.');
                </script>
            </head>
            <body>
                <h1>Welcome to the Test Page</h1>
                <p class='content'>This is a paragraph of text.</p>
                <div class='content'>This is a div with some content.</div>
                <div class='hidden'>This content should be hidden.</div>
                <script>
                    alert('Another test script.');
                </script>
            </body>
            </html>
            ";

            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(HtmlInput)
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var mockContentExtractor = new Mock<IContentExtractor>();
            mockContentExtractor
                .Setup(x => x.ExtractContentFromHtml(It.IsAny<string>()))
                .Returns("Welcome to the Test Page This is a paragraph of text. This is a div with some content.");

            var urlLoaderService = new UrlLoaderService(httpClient, mockContentExtractor.Object);

            var urls = new List<string> { "https://example.com" };

            // Act
            var result = await urlLoaderService.ProcessUrls(urls);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey("https://example.com"));
            Assert.Equal("Welcome to the Test Page This is a paragraph of text. This is a div with some content.", result["https://example.com"]);
        }

        [Fact]
        public async Task ProcessUrls_ReturnsError_ForInvalidUrls()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Throws(new HttpRequestException("Invalid URL"));

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var mockContentExtractor = new Mock<IContentExtractor>();

            var urlLoaderService = new UrlLoaderService(httpClient, mockContentExtractor.Object);

            var urls = new List<string> { "https://invalid-url" };

            // Act
            var result = await urlLoaderService.ProcessUrls(urls);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey("https://invalid-url"));
            Assert.StartsWith("Error:", result["https://invalid-url"]);
        }
    }
}
