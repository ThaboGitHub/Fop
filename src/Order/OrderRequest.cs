namespace Fop.Order;

public class OrderRequest : IOrderRequest
{
    public OrderRequest(string orderBy, OrderDirection direction)
    {
        OrderBy = orderBy;
        Direction = direction;
    }

    public string OrderBy { get; }
    public OrderDirection Direction { get; }
}