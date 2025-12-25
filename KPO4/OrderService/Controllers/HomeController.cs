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
    public IActionResult InputOrder([FromForm]Guid id, [FromForm]string description)
    {
        var order = new Order(id, description);
        var ans = _context.Orders.Find(id);
        if (ans != null)
        {
            return BadRequest($"Счёт с ID {id} уже есть.");
        }
        _context.Orders.Add(order);
        _context.SaveChanges();
        return Ok(new {
            ThisOrderId = order.OrderId}
            );
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