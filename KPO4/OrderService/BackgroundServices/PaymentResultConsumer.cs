using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderService.BackgroundServices;

public class PaymentResultConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PaymentResultConsumer> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IConfiguration _configuration;

    public PaymentResultConsumer(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<PaymentResultConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;

        var hostName = _configuration["RabbitMQ:HostName"] ?? "localhost";
        var factory = new ConnectionFactory { HostName = hostName, DispatchConsumersAsync = true };
        
        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "payment_results_queue", durable: true, exclusive: false, autoDelete: false);
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to connect to RabbitMQ in PaymentResultConsumer.");
            throw;
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderServiceDBContext>();
            var body = ea.Body.ToArray();
            var messageJson = Encoding.UTF8.GetString(body);

            try
            {
                var resultData = JsonSerializer.Deserialize<PaymentResultData>(messageJson);
                if (resultData != null)
                {
                    await UpdateOrderStatusAsync(dbContext, resultData);
                }
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment result message.");
            }
        };

        _channel.BasicConsume(queue: "payment_results_queue", autoAck: false, consumer: consumer);
        return Task.CompletedTask;
    }

    private async Task UpdateOrderStatusAsync(OrderServiceDBContext db, PaymentResultData data)
    {
        var order = await db.Orders.FindAsync(data.OrderId);
        if (order == null)
        {
            _logger.LogWarning("Payment result for unknown OrderId: {OrderId}", data.OrderId);
            return;
        }

        if (order.Status == OrderStatus.Finished || order.Status == OrderStatus.Cancelled)
        {
             _logger.LogInformation("Order {OrderId} is already final.", data.OrderId);
             return;
        }

        if (data.IsSuccess)
        {
            order.Status = OrderStatus.Finished;
            _logger.LogInformation("Order {OrderId} PAID.", data.OrderId);
        }
        else
        {
            order.Status = OrderStatus.Cancelled;
            _logger.LogWarning("Order {OrderId} FAILED. Reason: {Reason}", data.OrderId, data.Reason);
        }

        await db.SaveChangesAsync();
    }
    
    private class PaymentResultData
    {
        public Guid OrderId { get; set; }
        public bool IsSuccess { get; set; }
        public string? Reason { get; set; }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
