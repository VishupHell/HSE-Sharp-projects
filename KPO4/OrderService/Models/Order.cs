namespace OrderService.Models;

public class Order
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public string Description {get; init; }
    public OrderStatus Status {get; init; }

    public Order(Guid orderId, string description)
    {
        OrderId = Guid.NewGuid();
        CustomerId = orderId;
        Description = description;
        Status = OrderStatus.Created;
    }
}