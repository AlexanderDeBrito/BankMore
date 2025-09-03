using BankMore.TariffService.Domain.Events;
using KafkaFlow;
using KafkaFlow.Producers;
using System.Text.Json;

namespace BankMore.TariffService.Infrastructure.Messaging;

public class TariffProducer
{
    private readonly IMessageProducer _producer;

    public TariffProducer(IProducerAccessor accessor)
    {
        _producer = accessor.GetProducer("tariff-producer")
            ?? throw new InvalidOperationException("Producer 'tariff-producer' não foi encontrado.");
    }

    public async Task ProduceAsync(TariffMessage message)
    {
        var topic = "tarifacoes-realizadas"; // Tópico
        var messageValue = JsonSerializer.SerializeToUtf8Bytes(message);

        await _producer.ProduceAsync(
            topic,
            message.AccountId,
            messageValue
        );
    }
}
