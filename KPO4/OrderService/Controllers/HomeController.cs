using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;

namespace OrderService.Controllers;


[ApiController][Route("[controller]")]
public class HomeController : ControllerBase
{
    private readonly OrderServiceDBContext _context;
    private readonly IWebHostEnvironment _environment;

    public HomeController(OrderServiceDBContext context, IWebHostEnvironment env)
    {
        _context = context;
        _environment = env;
    }

    [HttpPost]
    public async Task<IActionResult> InputOrder([FromForm] Guid userId, [FromForm] string description)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try 
        {
            decimal amount = new Random().Next(100, 5000);

            var order = new Order(userId, description, amount);
        
            _context.Orders.Add(order);

            var eventData = new 
            { 
                OrderId = order.OrderId, 
                UserId = userId, 
                Amount = amount,
                Description = description 
            };

            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = "OrderCreated",
                Payload = System.Text.Json.JsonSerializer.Serialize(eventData),
                CreatedAt = DateTime.UtcNow
            };
            _context.OutboxMessages.Add(outboxMessage);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { OrderId = order.OrderId, Price = amount, Status = "Processing" });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return BadRequest(ex.Message);
        }
    }

    
    [HttpGet]
    public IActionResult GetUserOrders(Guid id)
    {
        List<Order> result = _context.Orders.Where(x => x.CustomerId == id).ToList();
        if (result == null)
        {
            return BadRequest($"Пользователь с ID {id} не создавал заказы.");
        }
        return Ok(new
        {
            results = result
        });
    }
    
    [HttpGet("order/status")]
    public IActionResult GetBankAccount(Guid OrderId)
    {
        Order result = _context.Orders.Find(OrderId);
        if (result == null)
        {
            return BadRequest($"Заказ с ID {OrderId} не найден.");
        }
        return Ok(new
        {
            status = result.Status,
        });
    }
}