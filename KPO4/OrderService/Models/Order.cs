namespace OrderService.Models;

public class Order
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public string Description {get; init; }
    public OrderStatus Status {get; set; }
    public decimal Amount { get; init; }
    public Order(Guid customerId, string description, decimal amount)
    {
        OrderId = Guid.NewGuid();
        CustomerId = customerId;
        Description = description;
        Status = OrderStatus.Accepted;
        Amount = amount;
    }
}