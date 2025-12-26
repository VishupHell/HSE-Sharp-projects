namespace PaymentsService.Models;

public class InboxMessage
{
    public Guid Id { get; set; }
    public DateTime ProcessedAt { get; set; }
}