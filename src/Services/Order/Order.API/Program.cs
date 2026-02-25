using Order.API;
using Order.Application;
using Order.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
// 各层以结构化方式注册其依赖关系，使用扩展方法实现分离。
// Infrastucture(基础设施层) - EF Core
// Application(应用层) - MediatR
// API(表示层) - Carter, Swagger, HealthChecks等
builder.Services.AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices();

var app = builder.Build();

// 配置HTTP请求管道。

app.Run();
