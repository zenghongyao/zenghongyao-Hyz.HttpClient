using System;
using System.Collections.Generic;
using System.Linq;

namespace Hyz.HttpClient
{
    /// <summary>
    /// 基础HTTP请求抽象实现
    /// </summary>
    /// <typeparam name="T">响应类型</typeparam>
    public abstract class BaseRequest<T> : IBaseRequest<T>
        where T : class
    {
        private string _requestApi = string.Empty;
        private IDictionary<string, string>? _headers;
        private IDictionary<string, string>? _queryParameters;
        private object? _body;

        /// <summary>
        /// HTTP方法
        /// </summary>
        public string Method { get; set; } = "POST";

        /// <summary>
        /// 获取请求API路径
        /// </summary>
        public string GetRequestApi()
        {
            // 如果有查询参数，自动拼接到URL中
            if (_queryParameters != null && _queryParameters.Count > 0)
            {
                var queryString = string.Join("&", _queryParameters.Select(kvp =>
                    $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

                var separator = _requestApi.Contains('?') ? "&" : "?";
                return $"{_requestApi}{separator}{queryString}";
            }

            return _requestApi;
        }

        /// <summary>
        /// 设置请求API路径
        /// </summary>
        public void SetRequestApi(string? path)
        {
            if (!string.IsNullOrEmpty(path))
                _requestApi = path!;
        }

        /// <summary>
        /// 获取请求头
        /// </summary>
        public IDictionary<string, string>? GetHeaders()
        {
            return _headers;
        }

        /// <summary>
        /// 添加单个请求头
        /// </summary>
        public void AddHeader(string key, string value)
        {
            _headers ??= new Dictionary<string, string>();
            _headers[key] = value;
        }

        /// <summary>
        /// 设置请求头
        /// </summary>
        public void SetHeaders(IDictionary<string, string>? headers)
        {
            if (headers == null || headers.Count == 0)
            {
                _headers = null;
                return;
            }

            _headers = new Dictionary<string, string>(headers);
        }

        /// <summary>
        /// 获取查询参数
        /// </summary>
        public IDictionary<string, string>? GetQueryParameters()
        {
            return _queryParameters;
        }

        /// <summary>
        /// 添加单个查询参数
        /// </summary>
        public void AddQueryParameter(string key, string value)
        {
            _queryParameters ??= new Dictionary<string, string>();
            _queryParameters[key] = value;
        }

        /// <summary>
        /// 设置查询参数
        /// </summary>
        public void SetQueryParameters(IDictionary<string, string>? parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                _queryParameters = null;
                return;
            }

            _queryParameters = new Dictionary<string, string>(parameters);
        }

        /// <summary>
        /// 获取请求体内容
        /// </summary>
        public object? GetBody()
        {
            return _body;
        }

        /// <summary>
        /// 设置请求体内容
        /// </summary>
        public void SetBody(object? body)
        {
            _body = body;
        }
    }
}
