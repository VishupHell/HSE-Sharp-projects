using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PaymentsService.Models;

namespace PaymentsService.Controllers;


[ApiController][Route("[controller]")]
public class HomeController : ControllerBase
{
    private readonly PaymentServiceDBContext _context;
    private readonly IWebHostEnvironment _environment;

    public HomeController(PaymentServiceDBContext context, IWebHostEnvironment env)
    {
        _context = context;
        _environment = env;
    }

    [HttpPost]
    public IActionResult InputBankAccount([FromForm]Guid id, [FromForm]string name, [FromForm]decimal amount)
    {
        var account = new BankAccount(id, name, amount);
        var ans = _context.Accounts.Find(id);
        if (ans != null)
        {
            return BadRequest($"Счёт с ID {id} уже есть.");
        }
        _context.Accounts.Add(account);
        _context.SaveChanges();
        return Ok();
    }
    
    [HttpGet]
    public IActionResult GetBankAccount(Guid id)
    {
        BankAccount result = _context.Accounts.Find(id);
        if (result == null)
        {
            return BadRequest($"Счёт с ID {id} не найден.");
        }
        return Ok(new
            {
                name = result.Name,
                amount = result.Amount,
            });
    }

    [HttpPost("balance")]
    public async Task<IActionResult> GetBalance([FromForm]Guid id, [FromForm]decimal amount)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account == null)
            return BadRequest($"Счёт с ID {id} не найден.");

        if (account.Amount + amount < 0)
        {
            return BadRequest($"На счете {id} недостаточно средстве.");
        }
        // Обновление баланса
        account.Amount += amount;

        // Сохранение в БД
        await _context.SaveChangesAsync();

        return Ok(new { account.Id, account.Amount });
    }
}