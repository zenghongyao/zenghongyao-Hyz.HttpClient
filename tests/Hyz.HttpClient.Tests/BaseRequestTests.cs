using System.Collections.Generic;
using Hyz.HttpClient;
using Xunit;

namespace Hyz.HttpClient.Tests
{
    /// <summary>
    /// BaseRequest测试
    /// </summary>
    public class BaseRequestTests
    {
        private class TestRequest : BaseRequest<object>
        {
        }

        [Fact]
        public void SetRequestApi_ShouldSetUrl()
        {
            // Arrange
            var request = new TestRequest();

            // Act
            request.SetRequestApi("/api/test");

            // Assert
            Assert.Equal("/api/test", request.GetRequestApi());
        }

        [Fact]
        public void AddHeader_ShouldAddHeader()
        {
            // Arrange
            var request = new TestRequest();

            // Act
            request.AddHeader("Authorization", "Bearer token");
            request.AddHeader("Content-Type", "application/json");

            // Assert
            var headers = request.GetHeaders();
            Assert.NotNull(headers);
            Assert.Equal(2, headers.Count);
            Assert.Equal("Bearer token", headers["Authorization"]);
            Assert.Equal("application/json", headers["Content-Type"]);
        }

        [Fact]
        public void SetHeaders_ShouldReplaceHeaders()
        {
            // Arrange
            var request = new TestRequest();
            request.AddHeader("OldHeader", "OldValue");

            // Act
            var newHeaders = new Dictionary<string, string>
            {
                { "NewHeader1", "Value1" },
                { "NewHeader2", "Value2" }
            };
            request.SetHeaders(newHeaders);

            // Assert
            var headers = request.GetHeaders();
            Assert.NotNull(headers);
            Assert.Equal(2, headers.Count);
            Assert.Equal("Value1", headers["NewHeader1"]);
            Assert.Equal("Value2", headers["NewHeader2"]);
        }

        [Fact]
        public void AddQueryParameter_ShouldAddParameter()
        {
            // Arrange
            var request = new TestRequest();
            request.SetRequestApi("/api/users");

            // Act
            request.AddQueryParameter("page", "1");
            request.AddQueryParameter("pageSize", "20");

            // Assert
            var url = request.GetRequestApi();
            Assert.Contains("page=1", url);
            Assert.Contains("pageSize=20", url);
        }

        [Fact]
        public void SetQueryParameters_ShouldReplaceParameters()
        {
            // Arrange
            var request = new TestRequest();
            request.SetRequestApi("/api/users");
            request.AddQueryParameter("old", "value");

            // Act
            var newParams = new Dictionary<string, string>
            {
                { "page", "2" },
                { "pageSize", "30" }
            };
            request.SetQueryParameters(newParams);

            // Assert
            var url = request.GetRequestApi();
            Assert.Contains("page=2", url);
            Assert.Contains("pageSize=30", url);
            Assert.DoesNotContain("old", url);
        }

        [Fact]
        public void GetRequestApi_ShouldAutoEncodeQueryParameters()
        {
            // Arrange
            var request = new TestRequest();
            request.SetRequestApi("/api/search");
            request.AddQueryParameter("keyword", "测试&搜索");

            // Act
            var url = request.GetRequestApi();

            // Assert
            Assert.Contains(Uri.EscapeDataString("测试&搜索"), url);
        }

        [Fact]
        public void GetRequestApi_ShouldHandleExistingQueryParameters()
        {
            // Arrange
            var request = new TestRequest();
            request.SetRequestApi("/api/users?status=active");
            request.AddQueryParameter("page", "1");

            // Act
            var url = request.GetRequestApi();

            // Assert
            Assert.Contains("status=active", url);
            Assert.Contains("&page=1", url);
            Assert.DoesNotContain("?page=1", url);
        }

        [Fact]
        public void SetBody_ShouldSetBody()
        {
            // Arrange
            var request = new TestRequest();
            var body = new { Id = 123, Name = "Test" };

            // Act
            request.SetBody(body);

            // Assert
            var retrievedBody = request.GetBody();
            Assert.NotNull(retrievedBody);
            Assert.Equal(body, retrievedBody);
        }

        [Fact]
        public void Method_ShouldDefaultToPost()
        {
            // Arrange & Act
            var request = new TestRequest();

            // Assert
            Assert.Equal("POST", request.Method);
        }

        [Fact]
        public void Method_ShouldBeSettable()
        {
            // Arrange
            var request = new TestRequest();

            // Act
            request.Method = "GET";

            // Assert
            Assert.Equal("GET", request.Method);
        }
    }
}
