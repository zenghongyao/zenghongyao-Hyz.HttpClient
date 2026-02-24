using System;
using System.Net;
using HttpClientType = System.Net.Http.HttpClient;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Hyz.HttpClient;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace Hyz.HttpClient.Tests
{
    /// <summary>
    /// HttpClientRequest测试
    /// </summary>
    public class HttpClientRequestTests
    {
        private class TestResponse
        {
            public int Code { get; set; }
            public bool Result => Code == 0;
            public string? Message { get; set; }
        }

        private class TestRequest : BaseRequest<TestResponse>
        {
        }

        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly Mock<ILogger<HttpClientRequest>> _mockLogger;
        private readonly HttpClientType _httpClient;

        public HttpClientRequestTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockLogger = new Mock<ILogger<HttpClientRequest>>();
            _httpClient = new HttpClientType(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://api.example.com")
            };
        }

        [Fact]
        public async Task ExecuteGetAsync_ShouldSendGetRequest()
        {
            // Arrange
            var request = new TestRequest();
            request.SetRequestApi("/api/test");

            var expectedResponse = new TestResponse { Code = 0, Message = "Success" };
            var responseContent = JsonSerializer.Serialize(expectedResponse);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var service = new HttpClientRequest(_mockLogger.Object,
                new TestHttpClientFactory(_httpClient),
                jsonSerializerOptions);

            // Act
            var response = await service.ExecuteGetAsync<TestResponse>(request);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Result);
            Assert.Equal("Success", response.Message);
        }

        [Fact]
        public async Task ExecutePostAsync_ShouldSendPostRequest()
        {
            // Arrange
            var request = new TestRequest();
            request.SetRequestApi("/api/test");
            request.SetBody(new { Name = "Test" });

            var expectedResponse = new TestResponse { Code = 0, Message = "Success" };
            var responseContent = JsonSerializer.Serialize(expectedResponse);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Post),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var service = new HttpClientRequest(_mockLogger.Object,
                new TestHttpClientFactory(_httpClient),
                jsonSerializerOptions);

            // Act
            var response = await service.ExecutePostAsync<TestResponse>(request);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Result);
        }

        [Fact]
        public async Task ExecutePutAsync_ShouldSendPutRequest()
        {
            // Arrange
            var request = new TestRequest();
            request.SetRequestApi("/api/test");
            request.SetBody(new { Name = "Updated" });

            var expectedResponse = new TestResponse { Code = 0 };
            var responseContent = JsonSerializer.Serialize(expectedResponse);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Put),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var service = new HttpClientRequest(_mockLogger.Object,
                new TestHttpClientFactory(_httpClient),
                jsonSerializerOptions);

            // Act
            var response = await service.ExecutePutAsync<TestResponse>(request);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Result);
        }

        [Fact]
        public async Task ExecuteDeleteAsync_ShouldSendDeleteRequest()
        {
            // Arrange
            var request = new TestRequest();
            request.SetRequestApi("/api/test/123");

            var expectedResponse = new TestResponse { Code = 0 };
            var responseContent = JsonSerializer.Serialize(expectedResponse);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Delete),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var service = new HttpClientRequest(_mockLogger.Object,
                new TestHttpClientFactory(_httpClient),
                jsonSerializerOptions);

            // Act
            var response = await service.ExecuteDeleteAsync<TestResponse>(request);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Result);
        }

        [Fact]
        public async Task ExecutePatchAsync_ShouldSendPatchRequest()
        {
            // Arrange
            var request = new TestRequest();
            request.SetRequestApi("/api/test/123");
            request.SetBody(new { Name = "Patched" });

            var expectedResponse = new TestResponse { Code = 0 };
            var responseContent = JsonSerializer.Serialize(expectedResponse);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r => r.Method.Method == "PATCH"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var service = new HttpClientRequest(_mockLogger.Object,
                new TestHttpClientFactory(_httpClient),
                jsonSerializerOptions);

            // Act
            var response = await service.ExecutePatchAsync<TestResponse>(request);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldIncludeHeaders()
        {
            // Arrange
            var request = new TestRequest();
            request.SetRequestApi("/api/test");
            request.AddHeader("Authorization", "Bearer test-token");
            request.AddHeader("X-Custom-Header", "custom-value");

            var expectedResponse = new TestResponse { Code = 0 };
            var responseContent = JsonSerializer.Serialize(expectedResponse);

            HttpRequestMessage? capturedRequest = null;

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, ct) =>
                {
                    capturedRequest = req;
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var service = new HttpClientRequest(_mockLogger.Object,
                new TestHttpClientFactory(_httpClient),
                jsonSerializerOptions);

            // Act
            await service.ExecuteGetAsync<TestResponse>(request);

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.True(capturedRequest.Headers.Contains("Authorization"));
            Assert.True(capturedRequest.Headers.Contains("X-Custom-Header"));
        }

        [Fact]
        public async Task ExecuteAsync_ShouldIncludeQueryParameters()
        {
            // Arrange
            var request = new TestRequest();
            request.SetRequestApi("/api/users");
            request.AddQueryParameter("page", "1");
            request.AddQueryParameter("pageSize", "20");

            var expectedResponse = new TestResponse { Code = 0 };
            var responseContent = JsonSerializer.Serialize(expectedResponse);

            HttpRequestMessage? capturedRequest = null;

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, ct) =>
                {
                    capturedRequest = req;
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var service = new HttpClientRequest(_mockLogger.Object,
                new TestHttpClientFactory(_httpClient),
                jsonSerializerOptions);

            // Act
            await service.ExecuteGetAsync<TestResponse>(request);

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.Contains("page=1", capturedRequest.RequestUri?.ToString() ?? string.Empty);
            Assert.Contains("pageSize=20", capturedRequest.RequestUri?.ToString() ?? string.Empty);
        }

        #region Test Helpers

        private class TestHttpClientFactory : IHttpClientFactory
        {
            private readonly HttpClientType _httpClient;

            public TestHttpClientFactory(HttpClientType httpClient)
            {
                _httpClient = httpClient;
            }

            public HttpClientType CreateClient(string? name = null)
            {
                return _httpClient;
            }
        }

        #endregion
    }
}
