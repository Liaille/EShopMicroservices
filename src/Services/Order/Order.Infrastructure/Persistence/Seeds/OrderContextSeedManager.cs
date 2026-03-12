namespace Order.Infrastructure.Persistence.Seeds;

public class OrderContextSeedManager(
    IEnumerable<ISeedData<OrderDbContext>> seedDatas,
    ILogger<OrderContextSeedManager> logger)
{
    /// <summary>
    /// 统一执行订单上下文所有种子数据，按照环境过滤、聚合名称分组、执行优先级排序后依次执行
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task ExecuteAllAsync(OrderDbContext context, CancellationToken cancellationToken = default)
    {
        // 根据当前环境过滤种子数据
        //var filteredSeedDatas = seedDatas.Where(seed => seed.ApplyEnvs.Contains(env.EnvironmentName, StringComparer.OrdinalIgnoreCase)).ToList();

        // 多级排序：聚合名称优先级 + 同聚合内执行优先级
        var sortedSeedDatas = seedDatas
            .OrderBy(s => s.AggregateName switch
            {
                AggregateNames.Customer => 1,
                AggregateNames.Product => 2,
                AggregateNames.Order => 3,
                _ => int.MaxValue
            })
            .ThenBy(s => s.ExecuteOrder);

        // 批量执行种子数据
        foreach (var seedData in sortedSeedDatas)
        {
            try
            {
                await seedData.ExecuteAsync(context, cancellationToken);
                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation("Successfully executed seed data: {TypeName} for aggregate: {AggregateName}", seedData.GetType().Name, seedData.AggregateName);
            }
            catch (Exception ex)
            {
                if (logger.IsEnabled(LogLevel.Error))
                    logger.LogError(ex, "Failed to execute seed data: {TypeName} for aggregate: {AggregateName}", seedData.GetType().Name, seedData.AggregateName);
            }
        }
    }
}
