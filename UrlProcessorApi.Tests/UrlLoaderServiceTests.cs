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
        public async Task ProcessUrls_ReturnsData_ForValidUrls()
        {
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
                    Content = new StringContent("<html><body><p>Test content</p><script>console.log('test');</script></body></html>")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var urlLoaderService = new UrlLoaderService(httpClient);

            var urls = new List<string> { "https://example.com" };

            // Act
            var result = await urlLoaderService.ProcessUrls(urls);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey("https://example.com"));
            Assert.Equal("Test content", result["https://example.com"]);
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
            var urlLoaderService = new UrlLoaderService(httpClient);

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
