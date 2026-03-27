using Order.API;
using Order.Application;
using Order.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 各层以结构化方式注册其依赖关系，使用扩展方法实现分离。
builder.Services.AddApplicationServices() // Application(应用层) - MediatR
    .AddInfrastructureServices(builder.Configuration, builder.Environment.IsDevelopment()) // Infrastucture(基础设施层) - EF Core
    .AddApiServices(builder.Configuration); // API(表示层) - Carter, Swagger, HealthChecks等

var app = builder.Build();

// 配置HTTP请求管道。
app.UseApiServices();

app.Run();
