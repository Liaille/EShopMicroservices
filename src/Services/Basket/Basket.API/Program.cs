using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
// 向容器中添加服务。
// 一个为ASP.NET Core最小API提供拓展功能和模块化的轻量级框架
// Carter包若安装在其他引用项目中则无法识别继承了ICarterModule的API
builder.Services.AddCarter();
// 注册MediatR中介类库
var assembly = typeof(Program).Assembly;
string dbConnectionString = builder.Configuration.GetConnectionString("Database")!;
string redisConnectionString = builder.Configuration.GetConnectionString("Redis")!;
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
// 注册全局异常处理
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// 注册PostgreSQL的Marten文档数据库
builder.Services.AddMarten(options =>
{
    // 配置Marten连接字符串
    options.Connection(dbConnectionString);
    // 因ShoppingCart没有Id属性，需手动配置主键
    options.Schema.For<ShoppingCart>().Identity(x => x.UserName);
}).UseLightweightSessions(); // 轻量级会话，移除了变更跟踪、身份映射缓存等机制
// 注册仓储服务
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
// 使用装饰器模式为仓储添加缓存功能(不使用Scrutor库时的配置方法)
//builder.Services.AddScoped<IBasketRepository>(sp =>
//{
//    var repository = sp.GetRequiredService<BasketRepository>();
//    var cache = sp.GetRequiredService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
//    return new CachedBasketRepository(repository, cache);
//});
// 使用Scrutor库为仓储添加缓存功能的装饰器模式
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();
// 注册Redis分布式缓存
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
});
// 注册健康检查服务
builder.Services.AddHealthChecks()
    .AddNpgSql(dbConnectionString)
    .AddRedis(redisConnectionString);

var app = builder.Build();
// 配置HTTP请求管道。
// 映射Carter模块中的路由
app.MapCarter();
// 使用全局异常处理中间件
// 此处因BUG，必须传入一个空的options委托，否则生成时会抛出异常
app.UseExceptionHandler(options => { });
// 配置健康检查端点
app.UseHealthChecks("/health", new HealthCheckOptions
{
    // 使用HealthCheckUIResponseWriter来格式化健康检查的响应
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
