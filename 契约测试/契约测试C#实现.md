# 契约测试C#实现

## 1. 术语

* 消费者（Consumer）:**对服务进行消费的代码**，通常指的是客户端
* 提供者（Provider）: **提供服务的代码**，通常指的是服务器端
* 契约（Contract）:消费者和提供者之间商定的协议。它包括预期的请求（输入）和响应（输出）

## 2. 为什么需要契约测试

* 构建和维护微服务是一项艰巨的任务，在众多服务必须彼此无缝交互的世界中，确保对一项服务的更改不会破坏另一项服务的功能是很让人头疼的

* 传统的集成测试针对的是整个系统之间的交互，工作量太大、速度太慢，甚至无法直接识别问题
* 契约测试侧重于测试各个服务之间的契约
* 合同测试根据消费者和提供商之间商定的契约分别对消费者和提供商进行测试

## 3. 如何执行契约测试

* 在契约测试中，消费者端程序员编写“消费者测试”，其中包含期望的输入和输出，并且期望将被保存到 `Pact Json` 文件中
* 运行时，测试将请求发送到内置的模拟服务器而不是真实服务器，模拟服务器使用保存的 `Pact Json` 文件发送响应，该响应将用于验证消费者端测试用例
* 此外，契约测试框架将读取保存的 `Pact Json` 文件，并向服务提供者（服务器）发送请求，并且将根据 `Pact Json` 文件中的预期输出来验证响应

## 4. What is Pact

* `Pact` 是合约测试的实现
* 由于消费者和提供者可能使用不同的编程语言进行开发，因此 `Pact` 是语言无关的，它支持多种编程语言，例如 `Java`、`.NET`、`Ruby`、`JavaScript`、`Python`、`PHP` 等
* 保存的 `Pact Json` 文件是由  用一种编程语言开发的消费者可以用来验证用另一种编程语言开发的提供者

## 5. 如何使用 Pact.Net

### 开发一个待测试的 WebAPI 服务

`Pact` 需要使用 `ASP.Net Core` 项目的 `Startup` 类来启动 `Web` 服务器

```c#
[ApiController]
[Route("[controller]/[action]")]
public class MyController : ControllerBase
{
    [HttpGet]
    public int Abs(int i)
    {
        return Math.Abs(i);
    }
}
```

### 编写消费者端测试用例

创建一个使用 `xUnit` 的 `.NET` 测试项目，然后在测试项目上安装 `PactNet` 这个 `Nuget` 包

```c#
public class UnitTest1
{
    private readonly IPactBuilderV4 pactBuilder;
    public UnitTest1()
    {
        var pact = Pact.V4("MyAPI consumer", "MyAPI",new PactConfig());
        this.pactBuilder = pact.WithHttpInteractions();
    }
    [Fact]
    public async Task Test1()
    {
        this.pactBuilder.UponReceiving("A request to calc Abs")
            .Given("Abs")
            .WithRequest(HttpMethod.Get, "/My/Abs")
            .WithQuery("i","-2")//Match.Integer(-2) 
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithJsonBody(2);

        await this.pactBuilder.VerifyAsync(async ctx=>
        {
            using HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = ctx.MockServerUri;
            var r = await httpClient.GetFromJsonAsync<int>($"/My/Abs?i=-2");
            Assert.Equal(2,r);
        });
    }
}
```

* `WithRequest().WithQuery()` 用于定义输入
* `WillRespond().WithJsonBody()` 用于定义相应的预期输出
* `VerifyAsync` 中的代码片段是测试用例，根据 `UponReceiving` 定义的期望进行测试
* 从 `httpClient.BaseAddress = ctx.MockServerUri` 可以看出，`Provider` 测试用例与 `Pact` 提供的 `Mock` 服务器交互而不是真实服务器进行交互
* 测试运行完成后，测试项目的 `pact` 文件夹下会生成一个 `MyAPI Consumer-MyAPI.json`，这个 `Json` 文件中保存了预期的输入和输出

### 编写提供者端测试用例

* 创建一个使用 `xUnit` 的 `.NET` 测试项目，然后向其安装 `Nuget` 包 `PactNet` 和 `PactNet.Output.Xunit`

* 由于提供程序测试必须使用 `Startup` 类启动测试服务器，因此请将待测试的 `ASP.NET Core WebAPI` 项目的引用添加到提供程序测试项目中

* 创建一个 `MyApiFixture` 类，用于启动测试项目中测试的 `WebAPI` 服务器

  ```c#
  public class MyApiFixture : IDisposable
  {
      private readonly IHost _server;
      public Uri ServerUri { get; }
      public MyApiFixture()
      {
          ServerUri = new Uri("http://localhost:9223");
          _server = Host.CreateDefaultBuilder()
              .ConfigureWebHostDefaults(webBuilder =>
              {
                  webBuilder.UseUrls(ServerUri.ToString());
                  webBuilder.UseStartup<Startup>();
              })
              .Build();
          _server.Start();
      }
  
      public void Dispose()
      {
          _server.Dispose();
      }
  }
  ```

* 接下来，如下创建一个使用保存的 `Pact Json` 文件对服务器（提供者）进行测试的测试用例

  ```c#
  public class UnitTest1 : IClassFixture<MyApiFixture>
  {
      private readonly MyApiFixture _fixture;
      private readonly ITestOutputHelper _output;
      public UnitTest1(MyApiFixture fixture, ITestOutputHelper output)
      {
          _fixture = fixture;
          _output = output;
      }
      [Fact]
      public async Task Test1()
      {
          var config = new PactVerifierConfig
          {
              Outputters = new List<IOutput>
              {
                  new XunitOutput(_output),
              },
          };
          string pactPath = Path.Combine("..", "..", "..", "..",
              "TestConsumerProject1", "pacts", "MyAPI consumer-MyAPI.json");
          using var pactVerifier = new PactVerifier("MyAPI", config);
          pactVerifier
              .WithHttpEndpoint(_fixture.ServerUri)
              .WithFileSource(new FileInfo(pactPath))
              .Verify();
      }
  }
  ```

  * `pactPath` 是指保存的 `Pact` 文件
  * 执行上述测试时，`Pact` 将启动测试项目中的测试服务器，发送请求并根据保存的 `Json` 文件验证响应

## 6. 对基于消息的服务使用 Pact

`Pact` 也支持对于基于消息的服务（也被称为 `async API`）进行测试。详细请查看 `Pact` 文档的 `messaging pacts` 部分