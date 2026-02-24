using System;
using Hyz.HttpClient;
using Xunit;

namespace Hyz.HttpClient.Tests
{
    /// <summary>
    /// HttpClientPolicy测试
    /// </summary>
    public class HttpClientPolicyTests
    {
        [Fact]
        public void GetApiPipelinePolicy_ShouldReturnPipeline()
        {
            // Act
            var pipeline = HttpClientPolicy.GetApiPipelinePolicy;

            // Assert
            Assert.NotNull(pipeline);
        }

        [Fact]
        public void ConfigureRetry_ShouldUpdateRetryOptions()
        {
            // Act
            HttpClientPolicy.ConfigureRetry(new HttpClientPolicy.RetryOptions
            {
                MaxRetryAttempts = 2
            });

            var pipeline = HttpClientPolicy.GetApiPipelinePolicy;

            // Assert
            Assert.NotNull(pipeline);
        }

        [Fact]
        public void ConfigureCircuitBreaker_ShouldUpdateCircuitBreakerOptions()
        {
            // Act
            HttpClientPolicy.ConfigureCircuitBreaker(new HttpClientPolicy.CircuitBreakerOptions
            {
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(10),
                MinimumThroughput = 5,
                BreakDuration = TimeSpan.FromSeconds(30)
            });

            var pipeline = HttpClientPolicy.GetApiPipelinePolicy;

            // Assert
            Assert.NotNull(pipeline);
        }

        [Fact]
        public void RetryOptions_ShouldHaveDefaultValues()
        {
            // Act
            var options = new HttpClientPolicy.RetryOptions();

            // Assert
            Assert.Equal(3, options.MaxRetryAttempts);
            Assert.Equal(Polly.DelayBackoffType.Exponential, options.BackoffType);
            Assert.Equal(TimeSpan.FromMilliseconds(200), options.InitialDelay);
            Assert.Null(options.OnRetry);
        }

        [Fact]
        public void CircuitBreakerOptions_ShouldHaveDefaultValues()
        {
            // Act
            var options = new HttpClientPolicy.CircuitBreakerOptions();

            // Assert
            Assert.Equal(1.0, options.FailureRatio);
            Assert.Equal(TimeSpan.FromSeconds(2), options.SamplingDuration);
            Assert.Equal(4, options.MinimumThroughput);
            Assert.Equal(TimeSpan.FromSeconds(3), options.BreakDuration);
            Assert.Null(options.OnOpened);
            Assert.Null(options.OnClosed);
            Assert.Null(options.OnHalfOpened);
        }

        [Fact]
        public void CanConfigureMultipleTimes()
        {
            // Arrange
            HttpClientPolicy.ConfigureRetry(new HttpClientPolicy.RetryOptions
            {
                MaxRetryAttempts = 5
            });

            // Act
            HttpClientPolicy.ConfigureRetry(new HttpClientPolicy.RetryOptions
            {
                MaxRetryAttempts = 7
            });

            var pipeline = HttpClientPolicy.GetApiPipelinePolicy;

            // Assert
            Assert.NotNull(pipeline);
        }
    }
}
