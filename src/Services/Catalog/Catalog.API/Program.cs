var builder = WebApplication.CreateBuilder(args);

// 向容器中添加服务。
// Carter包若安装在其他引用项目中则无法识别继承了ICarterModule的API
builder.Services.AddCarter(); // 一个为ASP.NET Core最小API提供拓展功能和模块化的轻量级框架
builder.Services.AddMediatR(config =>
{
    // 将此项目中的所有服务都注册到中介类库中
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
builder.Services.AddMarten(options =>
{
    // 配置Marten连接字符串
    options.Connection(builder.Configuration.GetConnectionString("Database")!);
}).UseLightweightSessions(); // 轻量级会话，移除了变更跟踪、身份映射缓存等机制

var app = builder.Build();

// 配置HTTP请求管道。
app.MapCarter();

app.Run();
