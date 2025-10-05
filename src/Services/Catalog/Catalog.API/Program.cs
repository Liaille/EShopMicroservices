var builder = WebApplication.CreateBuilder(args);
// 向容器中添加服务。
builder.Services.AddCarter(); // 一个为ASP.NET Core最小API提供拓展功能和模块化的轻量级框架
builder.Services.AddMediatR(config =>
{
    // 将此项目中的所有服务都注册到中介类库中
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

var app = builder.Build();

// 配置HTTP请求管道。
app.MapCarter();

app.Run();
