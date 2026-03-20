using BuildingBlocks.Exceptions;

namespace Order.Application.Exceptions;

public class OrderNotFoundException(Guid id) : NotFoundException("Order", id)
{
}
