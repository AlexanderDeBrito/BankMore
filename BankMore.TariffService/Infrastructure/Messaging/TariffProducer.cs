using BankMore.TariffService.Domain.Events;
using KafkaFlow;
using KafkaFlow.Producers;
using System.Text.Json;

namespace BankMore.TariffService.Infrastructure.Messaging;

public class TariffProducer
{
    private readonly IMessageProducer<TariffMessage> _producer;

    public TariffProducer(IProducerAccessor accessor)
    {
        _producer = (IMessageProducer<TariffMessage>?)accessor.GetProducer("tariff-producer");
        if (_producer == null)
            throw new InvalidOperationException("Producer 'tariff-producer' não foi encontrado.");
    }

    public async Task ProduceAsync(TariffMessage message)
    {
        var topic = "tarifacoes-realizadas"; // Tópico configurado        
        var messageValue = JsonSerializer.SerializeToUtf8Bytes(message);      
        var deliveryResult = await _producer.ProduceAsync(topic, message.AccountId, messageValue, null, null);
        
    }
}
