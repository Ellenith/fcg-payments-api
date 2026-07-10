
# FIAP Cloud Gaming - Fase 02

Tech Challenge Pos-graduacao FIAP.

# FCG — Payments API

Microsserviço de Pagamentos da FIAP Cloud Games. Responsável por
processar (simular) o pagamento de uma compra de jogo.

Parte da Fase 2 do Tech Challenge — arquitetura de microsserviços
orientada a eventos.

---

## Responsabilidades

- Consome `OrderPlacedEvent` publicado pelo CatalogAPI
- Simula o processamento de pagamento (sem gateway real)
- Publica `PaymentProcessedEvent` com o resultado (`Approved` ou
  `Rejected`)

---

## Características

Este é o serviço mais simples da arquitetura — **sem banco de dados e
sem endpoints de negócio**. Ele existe apenas para consumir um evento,
processar e publicar outro.

Simulação de aprovação: **90% de chance de aprovar**, 10% de rejeitar
— usado para demonstrar ambos os caminhos do fluxo no vídeo de
apresentação.

---

## Tecnologias

- .NET 9
- MassTransit + RabbitMQ (consome e publica eventos)
- Serilog (logs em arquivo)

---

## Endpoints

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/health` | Health check para o Kubernetes |

> Este serviço não expõe endpoints de negócio — toda a interação
> acontece via mensageria.

---

## Eventos consumidos

### `OrderPlacedEvent`

```csharp
public record OrderPlacedEvent(
    Guid     OrderId,
    Guid     UsuarioId,
    Guid     JogoId,
    string   TituloJogo,
    decimal  Preco,
    DateTime CriadoEm);
```

## Eventos publicados

### `PaymentProcessedEvent`

```csharp
public record PaymentProcessedEvent(
    Guid     OrderId,
    Guid     UsuarioId,
    Guid     JogoId,
    decimal  Preco,
    string   Status,     // "Approved" ou "Rejected"
    string   Mensagem,
    DateTime ProcessadoEm);
```

---

## Variáveis de ambiente

| Variável | Descrição | Exemplo |
|----------|-----------|---------|
| `RabbitMQ__Host` | Host do RabbitMQ | `rabbitmq` (Docker) ou `localhost` |
| `RabbitMQ__Usuario` | Usuário do RabbitMQ | `guest` |
| `RabbitMQ__Senha` | Senha do RabbitMQ | `guest` |

---

## Como rodar localmente

**Pré-requisitos:**
- .NET 9 SDK
- RabbitMQ rodando (local ou via Docker)

```bash
dotnet restore
dotnet run --project FCG.PaymentsAPI
```

O serviço não expõe Swagger — verifique se está rodando acessando
`http://localhost:{porta}/health`.

---

## Como rodar com Docker

```bash
docker build -t fcg-payments-api .
docker run -p 5003:8080 \
  -e RabbitMQ__Host=host.docker.internal \
  fcg-payments-api
```

---

## Observando o processamento

Como não há endpoints de negócio, o processamento é observado via logs:

```bash
docker logs fcg-payments-api -f
```

Cada pagamento processado gera uma linha de log com o `OrderId`, o
valor e o status do pagamento (`Approved` ou `Rejected`).

---

*FIAP Cloud Games · Tech Challenge Fase 2 · PosTech FIAP*