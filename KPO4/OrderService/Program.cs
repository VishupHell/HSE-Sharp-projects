using Microsoft.EntityFrameworkCore;

namespace OrderService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<OrderServiceDBContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("Connection")));

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHostedService<OutboxPublisher>();
        builder.Services.AddHostedService<OrderService.BackgroundServices.PaymentResultConsumer>();
        builder.Services.AddCors();
        var app = builder.Build();

// Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
    
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrderServiceDBContext>();
            if (db.Database.IsRelational())
            {
                db.Database.Migrate();
            }
        }

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}