using System.Diagnostics;

namespace Order.Infrastructure.Persistence.Extensions;

/// <summary>
/// 为 <see cref="Activity"/> 提供异常标签相关的扩展方法，遵循 OpenTelemetry（OTel）异常语义规范。
/// </summary>
/// <remarks>
/// 参考 OTel 异常语义约定：https://opentelemetry.io/docs/specs/otel/trace/semantic_conventions/exceptions/
/// 核心作用：将 .NET 异常信息标准化为 OTel 追踪标签，便于分布式追踪平台（如 Jaeger、Zipkin）统一展示异常上下文。
/// </remarks>
public static class ActivityExtensions
{
    /// <summary>
    /// 为 <see cref="Activity"/> 设置符合 OTel 规范的异常标签，并标记 Activity 状态为错误。
    /// </summary>
    /// <param name="activity">待设置标签的追踪活动实例，可为 null（null 时直接返回，无异常）。</param>
    /// <param name="ex">需要记录的异常实例，不能为空引用（调用方需确保传入有效异常）。</param>
    /// <remarks>
    /// 已标准化的 OTel 标签说明：
    /// <list type="bullet">
    /// <item><c>exception.type</c>：异常的完整类型名（如 System.FormatException）；</item>
    /// <item><c>exception.message</c>：异常的消息文本；</item>
    /// <item><c>exception.stacktrace</c>：包含异常类型、消息、完整堆栈的原生字符串（ex.ToString()）；</item>
    /// </list>
    /// </remarks>
    public static void SetExceptionTags(this Activity activity, Exception ex)
    {
        if (activity is null) return;

        activity.AddTag("exception.type", ex.GetType().FullName);
        activity.AddTag("exception.message", ex.Message);
        activity.AddTag("exception.stacktrace", ex.ToString());
        activity.SetStatus(ActivityStatusCode.Error);
    }
}
