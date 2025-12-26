namespace PaymentsService.Models;

public class BankAccount
{
    public Guid Id { get; init; }
    
    public string Name { get; init; }
    
    public decimal Amount { get; set; }

    public BankAccount(Guid id, string name, decimal amount)
    {
        Id = id;
        Name = name;
        Amount = amount;
    }
    public BankAccount() : this(Guid.Empty, "", 0) { }
}