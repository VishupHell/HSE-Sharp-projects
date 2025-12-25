namespace OrderService.Models;

public enum OrderStatus
{
    Created,
    Paid,
    Abandoned //На случай, если на счете не хватило денег, или счет не был найден
}