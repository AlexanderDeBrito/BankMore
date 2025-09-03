using BankMore.TransferService.Domain.Events; // Ajuste o namespace conforme necessário
using KafkaFlow;
using KafkaFlow.Producers;
using System.Text.Json;

namespace BankMore.TransferService.Infrastructure.Messaging;

public class TransferProducer
{
    private readonly IMessageProducer<TransferMessage> _producer;

    public TransferProducer(IProducerAccessor accessor)
    {
        _producer = (IMessageProducer<TransferMessage>?)accessor.GetProducer("transfer-producer");
        if (_producer == null) throw new InvalidOperationException("Producer 'transfer-producer' não foi encontrado.");
    }

    public async Task ProduceAsync(TransferMessage message)
    {
        var topic = "tarifacoes-realizadas";       
        var messageValue = JsonSerializer.SerializeToUtf8Bytes(message);
        var deliveryResult = await _producer.ProduceAsync(topic, message.AccountId, messageValue, null, null);

        
    }
}