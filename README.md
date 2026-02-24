# Hyz.HttpClient

> 优雅的 HttpClient 封装，让你的 API 调用更加丝滑！

## ✨ 特性

- 🚀 **多种 HTTP 方法支持**：GET、POST、PUT、DELETE、PATCH
- 🔄 **自动重试机制**：支持指数退避，可配置重试次数
- ⚡ **熔断保护**：防止雪崩效应，支持自动恢复
- 🎯 **灵活的请求管理**：请求头、查询参数、请求体统一管理
- 📦 **类型安全**：强类型的请求和响应
- 🔒 **线程安全**：策略缓存优化，支持并发配置更新
- 🎨 **优雅的 API 设计**：简单易用，开箱即用

## 📦 安装

```bash
dotnet add package Hyz.HttpClient
```

## 🚀 快速开始

### 1. 注册服务

```csharp
using Hyz.HttpClient;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// 方式1：使用默认配置
services.AddHyzHttpClient();

// 方式2：自定义HttpClient名称
services.AddHyzHttpClient("MyApi");

// 方式3：配置HttpClient
services.AddHyzHttpClient("MyApi", client =>
{
    client.BaseAddress = new Uri("https://api.example.com");
    client.Timeout = TimeSpan.FromSeconds(30);
});

var serviceProvider = services.BuildServiceProvider();
```

### 2. 注入并使用

```csharp
public class UserService
{
    private readonly HttpClientRequest _httpClientService;

    public UserService(HttpClientRequest httpClientService)
    {
        _httpClientService = httpClientService;
    }

    // GET 请求
    public async Task<List<User>?> GetUsersAsync(int page = 1, int pageSize = 20)
    {
        var request = new BaseApiRequest<UserListResponse>();
        request.SetRequestApi("/api/users");

        // 添加查询参数
        request.AddQueryParameter("page", page.ToString());
        request.AddQueryParameter("pageSize", pageSize.ToString());

        var response = await _httpClientService.ExecuteGetAsync<UserListResponse>(request);
        return response?.Result == true ? response.Users : null;
    }

    // POST 请求
    public async Task<User?> CreateUserAsync(CreateUserDto userDto)
    {
        var request = new BaseApiRequest<UserResponse>();
        request.SetRequestApi("/api/users");

        // 设置请求体
        request.SetBody(userDto);

        var response = await _httpClientService.ExecutePostAsync<UserResponse>(request);
        return response?.Result == true ? response.User : null;
    }

    // PUT 请求
    public async Task<bool> UpdateUserAsync(int userId, UpdateUserDto userDto)
    {
        var request = new BaseApiRequest<BaseApiResponse>();
        request.SetRequestApi($"/api/users/{userId}");

        request.SetBody(userDto);

        var response = await _httpClientService.ExecutePutAsync<BaseApiResponse>(request);
        return response?.Result == true;
    }

    // DELETE 请求
    public async Task<bool> DeleteUserAsync(int userId)
    {
        var request = new BaseApiRequest<BaseApiResponse>();
        request.SetRequestApi($"/api/users/{userId}");

        var response = await _httpClientService.ExecuteDeleteAsync<BaseApiResponse>(request);
        return response?.Result == true;
    }

    // PATCH 请求
    public async Task<bool> PatchUserAsync(int userId, Dictionary<string, object> updates)
    {
        var request = new BaseApiRequest<BaseApiResponse>();
        request.SetRequestApi($"/api/users/{userId}");

        request.SetBody(updates);

        var response = await _httpClientService.ExecutePatchAsync<BaseApiResponse>(request);
        return response?.Result == true;
    }
}
```

### 3. 自定义请求类

```csharp
// 继承 BaseApiRequest 创建自己的请求类
public class LoginRequest : BaseApiRequest<LoginResponse>
{
    public LoginInfo? Login { get; set; }
}

public class LoginInfo
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public class LoginResponse : BaseApiResponse
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
}

// 使用自定义请求类
public async Task<string?> LoginAsync(string username, string password)
{
    var request = new LoginRequest();
    request.SetRequestApi("/api/login");
    request.Login = new LoginInfo
    {
        Username = username,
        Password = password
    };

    var response = await _httpClientService.ExecutePostAsync<LoginResponse>(request);
    return response?.Result == true ? response.Token : null;
}
```

## 📝 高级用法

### 配置重试策略

```csharp
using Hyz.HttpClient;

// 配置重试选项
HttpClientPolicy.ConfigureRetry(new HttpClientPolicy.RetryOptions
{
    MaxRetryAttempts = 5,  // 重试5次
    BackoffType = DelayBackoffType.Exponential,  // 指数退避
    InitialDelay = TimeSpan.FromMilliseconds(500),  // 初始延迟500ms
    OnRetry = args =>
    {
        Console.WriteLine($"重试第 {args.AttemptNumber} 次");
        return default;
    }
});
```

### 配置熔断策略

```csharp
// 配置熔断选项
HttpClientPolicy.ConfigureCircuitBreaker(new HttpClientPolicy.CircuitBreakerOptions
{
    FailureRatio = 0.5,  // 失败率达到50%时熔断
    SamplingDuration = TimeSpan.FromSeconds(10),  // 采样窗口10秒
    MinimumThroughput = 10,  // 最小吞吐量10次
    BreakDuration = TimeSpan.FromSeconds(30),  // 熔断持续时间30秒
    OnOpened = args => Console.WriteLine("熔断已打开"),
    OnClosed = args => Console.WriteLine("熔断已关闭"),
    OnHalfOpened = args => Console.WriteLine("熔断半开状态")
});
```

### 使用请求头

```csharp
var request = new BaseApiRequest<UserListResponse>();

// 添加单个请求头
request.AddHeader("Authorization", "Bearer token123");
request.AddHeader("Content-Type", "application/json");
request.AddHeader("X-Request-ID", Guid.NewGuid().ToString());

// 批量设置请求头
var headers = new Dictionary<string, string>
{
    { "X-Client-Version", "1.0.0" },
    { "X-Platform", "Web" }
};
request.SetHeaders(headers);
```

### 使用查询参数

```csharp
var request = new BaseApiRequest<UserListResponse>();
request.SetRequestApi("/api/users");

// 添加查询参数
request.AddQueryParameter("page", "1");
request.AddQueryParameter("pageSize", "20");
request.AddQueryParameter("status", "active");

// 批量设置查询参数
var queryParams = new Dictionary<string, string>
{
    { "keyword", "john" },
    { "sort", "name" },
    { "order", "asc" }
};
request.SetQueryParameters(queryParams);

// URL 自动拼接为：/api/users?page=1&pageSize=20&status=active
```

### 禁用重试

```csharp
// 对于非幂等性操作，可以禁用重试
var response = await _httpClientService.ExecutePostAsync<CreateUserResponse>(
    request,
    enableRetry: false
);
```

## 🎯 API 参考

### HttpClientRequest

| 方法 | 说明 |
|------|------|
| `ExecuteGetAsync<T>()` | 发送 GET 请求 |
| `ExecutePostAsync<T>()` | 发送 POST 请求 |
| `ExecutePutAsync<T>()` | 发送 PUT 请求 |
| `ExecuteDeleteAsync<T>()` | 发送 DELETE 请求 |
| `ExecutePatchAsync<T>()` | 发送 PATCH 请求 |
| `ExecuteAsync<T>()` | 通用方法，支持任意 HTTP 方法 |

### BaseApiRequest<T>

| 属性/方法 | 说明 |
|-----------|------|
| `SetRequestApi(string path)` | 设置 API 路径 |
| `GetRequestApi()` | 获取 API 路径（自动拼接查询参数） |
| `AddHeader(key, value)` | 添加单个请求头 |
| `SetHeaders(dictionary)` | 批量设置请求头 |
| `GetHeaders()` | 获取请求头字典 |
| `AddQueryParameter(key, value)` | 添加单个查询参数 |
| `SetQueryParameters(dictionary)` | 批量设置查询参数 |
| `GetQueryParameters()` | 获取查询参数字典 |
| `SetBody(object)` | 设置请求体 |
| `GetBody()` | 获取请求体对象 |
| `Method` | HTTP 方法（GET/POST/PUT/DELETE/PATCH） |


## 💡 最佳实践

### 1. 合理配置重试次数

```csharp
// 建议：3-5 次
HttpClientPolicy.ConfigureRetry(new HttpClientPolicy.RetryOptions
{
    MaxRetryAttempts = 3
});
```

### 2. 选择合适的退避策略

```csharp
// 指数退避通常是最佳选择
BackoffType = DelayBackoffType.Exponential
```

### 3. 设置合理的熔断参数

```csharp
// 根据业务特点调整
FailureRatio = 0.5,           // 失败率阈值 0.5-0.8
SamplingDuration = 10s,      // 采样窗口 10-30 秒
MinimumThroughput = 10,      // 最小吞吐量 5-10
BreakDuration = 30s          // 熔断时长 30-60 秒
```

### 4. 使用请求头追踪

```csharp
request.AddHeader("X-Request-ID", Guid.NewGuid().ToString());
```

### 5. HTTP 方法选择建议

| 方法 | 用途 | 场景 |
|------|------|------|
| GET | 获取资源 | 查询数据、列表、详情 |
| POST | 创建资源 | 新增记录、提交表单 |
| PUT | 完整更新 | 更新整个资源 |
| PATCH | 部分更新 | 更新资源的部分字段 |
| DELETE | 删除资源 | 删除记录 |


## 📄 许可证

MIT License - 详见 [LICENSE](LICENSE) 文件

**如果这个项目对你有帮助，请给它一个 ⭐️**
