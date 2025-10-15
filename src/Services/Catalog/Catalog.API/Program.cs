using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using Catalog.API.Data;

var builder = WebApplication.CreateBuilder(args);

// 向容器中添加服务。
// 注册MediatR中介类库
var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    // 将此项目中的所有服务都注册到中介类库中
    config.RegisterServicesFromAssembly(assembly);
    // 将验证行为添加到MediatR的管道中
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    // 将日志行为添加到MediatR的管道中
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
// 注册FluentValidation验证器
builder.Services.AddValidatorsFromAssembly(assembly);
// 一个为ASP.NET Core最小API提供拓展功能和模块化的轻量级框架
// Carter包若安装在其他引用项目中则无法识别继承了ICarterModule的API
builder.Services.AddCarter(); 
// 注册PostgreSQL的Marten文档数据库
builder.Services.AddMarten(options =>
{
    // 配置Marten连接字符串
    options.Connection(builder.Configuration.GetConnectionString("Database")!);
}).UseLightweightSessions(); // 轻量级会话，移除了变更跟踪、身份映射缓存等机制
// 注册全局异常处理
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

if (builder.Environment.IsDevelopment())
{
    // 在开发环境中，初始化Marten数据库并添加种子数据
    builder.Services.InitializeMartenWith<CatalogInitialData>();
}

var app = builder.Build();

// 配置HTTP请求管道。
app.MapCarter();
// 使用全局异常处理中间件
// 此处因BUG，必须传入一个空的options委托，否则生成时会抛出异常
app.UseExceptionHandler(options => { });

app.Run();
