using BankMore.TariffService.Domain.Entities;
using BankMore.TariffService.Domain.Events;
using BankMore.TariffService.Infrastructure.Persistence;
using KafkaFlow;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankMore.TariffService.Infrastructure.Messaging;

public class TransferConsumer : IMessageHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly TariffProducer _producer;

    public TransferConsumer(IServiceProvider serviceProvider, IConfiguration configuration, TariffProducer producer)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _producer = producer;
    }

    public async Task Handle(IMessageContext context)
    {
        // para contornar a criação de instancia dentro kafka
        using var scope = _serviceProvider.CreateScope();

        var repo = scope.ServiceProvider.GetRequiredService<TariffRepository>();

        var message = context.Message.Value as Dictionary<string, object> ??
                      throw new InvalidOperationException("Mensagem inválida");

        if (!message.TryGetValue("AccountId", out var accountIdObj) || !int.TryParse(accountIdObj?.ToString(), out int accountId))
        {
            throw new InvalidOperationException("AccountId ausente ou inválido na mensagem");
        }

        var tariffValue = _configuration.GetValue<decimal>("TariffValue", 2.00m);
        var tariff = new Tariff { AccountId = accountId, Value = tariffValue };

        await repo.CreateAsync(tariff);
        await _producer.ProduceAsync(new TariffMessage { AccountId = accountId, Valor = tariffValue });
    }
}
