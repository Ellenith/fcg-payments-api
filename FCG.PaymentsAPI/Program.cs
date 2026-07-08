// ============================================================
// Program.cs — PaymentsAPI
// Microsserviço de Pagamentos da FIAP Cloud Games
// Sem endpoints HTTP — apenas consome e publica eventos
// ============================================================

using FCG.PaymentsAPI.Consumidores;
using MassTransit;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/payments-api-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddMassTransit(x =>
{
    // Registra o consumidor do OrderPlacedEvent
    x.AddConsumer<OrderPlacedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Usuario"]!);
            h.Password(builder.Configuration["RabbitMQ:Senha"]!);
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Health check simples para o Kubernetes verificar se o serviço está vivo
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Service = "PaymentsAPI" }));

app.Run();