using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PaymentsService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PaymentsService.BackgroundServices;

public class OrderConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IConfiguration _configuration;

    public OrderConsumer(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        
        var hostName = _configuration["RabbitMQ:HostName"] ?? "localhost";

        var factory = new ConnectionFactory { HostName = hostName, DispatchConsumersAsync = true };
        
         _connection = factory.CreateConnection();
         _channel = _connection.CreateModel();
         _channel.QueueDeclare(queue: "orders_queue", durable: true, exclusive: false, autoDelete: false);
         _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentServiceDBContext>();

            var body = ea.Body.ToArray();
            var messageJson = Encoding.UTF8.GetString(body);
            
            try 
            {
                var orderEvent = JsonSerializer.Deserialize<OrderEventData>(messageJson);
                
                if (orderEvent != null)
                {
                    await ProcessOrderAsync(dbContext, orderEvent);
                }

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
                _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false); 
                
            }
        };

        _channel.BasicConsume(queue: "orders_queue", autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }

    private async Task ProcessOrderAsync(PaymentServiceDBContext db, OrderEventData data)
    {
        if (await db.InboxMessages.AnyAsync(m => m.Id == data.OrderId)) return;

        using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            var account = await db.Accounts.FindAsync(data.UserId);

            bool isSuccess = false;
            string reason = "";

            if (account == null)
            {
                isSuccess = false;
                reason = "Account not found";
                Console.WriteLine($"[Payment] Failed. Account {data.UserId} not found.");
            }
            else if (account.Amount < data.Amount)
            {
                isSuccess = false;
                reason = "Insufficient funds";
                Console.WriteLine($"[Payment] Failed. Order {data.OrderId}. No money.");
            }
            else
            {
                account.Amount -= data.Amount;
                isSuccess = true;
                Console.WriteLine($"[Payment] Success. Order {data.OrderId} paid.");
            }

            var resultEvent = new
            {
                OrderId = data.OrderId,
                IsSuccess = isSuccess,
                Reason = reason
            };

            db.OutboxMessages.Add(new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = isSuccess ? "PaymentSucceeded" : "PaymentFailed",
                Payload = JsonSerializer.Serialize(resultEvent),
                CreatedAt = DateTime.UtcNow
            });

            db.InboxMessages.Add(new InboxMessage { Id = data.OrderId, ProcessedAt = DateTime.UtcNow });

            await db.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    
    private class OrderEventData
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
