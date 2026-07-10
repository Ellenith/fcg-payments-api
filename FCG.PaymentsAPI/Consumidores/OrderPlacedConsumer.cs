// ============================================================
// OrderPlacedConsumer.cs — Consumidor do PaymentsAPI
//
// Consome OrderPlacedEvent publicado pelo CatalogAPI.
// Simula o processamento do pagamento e publica o resultado
// em PaymentProcessedEvent com status Approved ou Rejected.
//
// Simulação: 90% de aprovação para demonstração.
// ============================================================

using FCG.Contratos;
using MassTransit;

namespace FCG.PaymentsAPI.Consumidores;

public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<OrderPlacedConsumer> _logger;

    public OrderPlacedConsumer(
        IPublishEndpoint publishEndpoint,
        ILogger<OrderPlacedConsumer> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger          = logger;
    }

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        var evento = context.Message;

        _logger.LogInformation(
            "Processando pagamento: OrderId={OrderId} Jogo={Titulo} Valor=R${Preco}",
            evento.OrderId,
            evento.TituloJogo,
            evento.Preco);

        // Simula tempo de processamento do pagamento
        await Task.Delay(500);

        // Simula aprovação — 90% de chance de aprovar
        var aprovado = new Random().Next(1, 11) <= 9;
        //var aprovado = false; // Para teste, sempre rejeita o pagamento
        var status   = aprovado ? "Approved" : "Rejected";
        var mensagem = aprovado
            ? $"Pagamento de R${evento.Preco:F2} aprovado para o jogo '{evento.TituloJogo}'."
            : "Pagamento recusado. Tente novamente.";

        _logger.LogInformation(
            "Pagamento {Status}: OrderId={OrderId}",
            status,
            evento.OrderId);

        // Publica o resultado para CatalogAPI e NotificationsAPI
        await _publishEndpoint.Publish(new PaymentProcessedEvent(
            evento.OrderId,
            evento.UsuarioId,
            evento.JogoId,
            evento.Preco,
            status,
            mensagem,
            DateTime.UtcNow));
    }
}