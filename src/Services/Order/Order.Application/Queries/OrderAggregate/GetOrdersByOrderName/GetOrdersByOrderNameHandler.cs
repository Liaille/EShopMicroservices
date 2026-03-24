namespace Order.Application.Queries.OrderAggregate.GetOrdersByOrderName;

public class GetOrdersByOrderNameHandler(IOrderDbContext dbContext) : IQueryHandler<GetOrdersByOrderNameQuery, GetOrdersByOrderNameResult>
{
    public async Task<GetOrdersByOrderNameResult> Handle(GetOrdersByOrderNameQuery query, CancellationToken cancellationToken)
    {
        var orders = await dbContext.Orders
            .Include(o => o.OrderItems)
            .AsNoTracking()
            .Where(o => o.OrderName.Value.Contains(query.OrderName))
            .OrderBy(o => o.OrderName)
            .Select(o => o.ToOrderDto())
            .ToListAsync(cancellationToken);

        return new GetOrdersByOrderNameResult(orders);
    }
}
