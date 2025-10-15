var builder = WebApplication.CreateBuilder(args);

// 向容器中添加服务。
var app = builder.Build();


// 配置HTTP请求管道。

app.Run();
