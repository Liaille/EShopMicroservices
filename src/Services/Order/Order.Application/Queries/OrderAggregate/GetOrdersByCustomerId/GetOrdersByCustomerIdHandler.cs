namespace Order.Application.Queries.OrderAggregate.GetOrdersByCustomerId;

public class GetOrdersByCustomerIdHandler(IOrderDbContext dbContext) : IQueryHandler<GetOrdersByCustomerIdQuery, GetOrdersByCustomerIdResult>
{
    public async Task<GetOrdersByCustomerIdResult> Handle(GetOrdersByCustomerIdQuery query, CancellationToken cancellationToken)
    {
        var orders = await dbContext.Orders
            .Include(o => o.OrderItems)
            .AsNoTracking()
            .Where(o => o.CustomerId == CustomerId.Create(query.CustomerId))
            .OrderBy(o => o.OrderName.Value)
            .Select(o => o.ToOrderDto())
            .ToListAsync(cancellationToken);

        return new GetOrdersByCustomerIdResult(orders);
    }
}
