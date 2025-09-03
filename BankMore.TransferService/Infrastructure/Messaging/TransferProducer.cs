using BankMore.TransferService.Domain.Events;
using KafkaFlow;
using KafkaFlow.Producers;
using System.Text.Json;

namespace BankMore.TransferService.Infrastructure.Messaging;

public class TransferProducer
{
    private readonly IMessageProducer _producer;

    public TransferProducer(IProducerAccessor accessor)
    {
        _producer = accessor.GetProducer("transfer-producer")
            ?? throw new InvalidOperationException("Producer 'transfer-producer' não foi encontrado.");
    }

    public async Task ProduceAsync(TransferMessage message)
    {
        var topic = "tarifacoes-realizadas";
        var messageValue = JsonSerializer.SerializeToUtf8Bytes(message);

        await _producer.ProduceAsync(
            topic,
            message.AccountId,
            messageValue
        );
    }
}
