using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;

namespace Hyz.HttpClient
{
    /// <summary>
    /// Hyz.HttpClient服务集合扩展
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// 添加Hyz.HttpClient服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="httpClientName">HttpClient名称（可选）</param>
        /// <param name="configureClient">配置HttpClient（可选）</param>
        /// <param name="configureJsonSerializer">配置JsonSerializerOptions（可选）</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddHyzHttpClient(
            this IServiceCollection services,
            string? httpClientName = null,
            Action<System.Net.Http.HttpClient>? configureClient = null,
            Action<JsonSerializerOptions>? configureJsonSerializer = null)
        {
            // 使用Microsoft的AddHttpClient方法注册HttpClientFactory
            if (string.IsNullOrEmpty(httpClientName))
            {
                services.AddHttpClient();
            }
            else if (configureClient != null)
            {
                services.AddHttpClient(httpClientName!, configureClient);
            }
            else
            {
                services.AddHttpClient(httpClientName!);
            }

            // 创建JsonSerializerOptions实例
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // 应用用户配置的JsonSerializerOptions
            configureJsonSerializer?.Invoke(jsonSerializerOptions);

            // 注册HttpClientRequest服务
            services.AddSingleton<HttpClientRequest>(sp =>
            {
                var factory = sp.GetRequiredService<IHttpClientFactory>();
                var logger = sp.GetRequiredService<ILogger<HttpClientRequest>>();
                return new HttpClientRequest(logger, factory, jsonSerializerOptions);
            });

            return services;
        }
    }
}
