using BankMore.AccountService.Application.Commands;
using BankMore.AccountService.Domain.Events;
using KafkaFlow;
using MediatR;

public class TariffConsumer : IMessageHandler<TariffMessage>
{
    private readonly IMediator _mediator;

    public TariffConsumer(IMediator mediator) => _mediator = mediator;

    public async Task Handle(IMessageContext context, TariffMessage message)
    {
        var command = new CreateMovementCommand(
            idempotencyKey: Guid.NewGuid().ToString(),  
            accountNumber: message.AccountNumber.ToString(), 
            value: message.Valor,
            type: "D", 
            loggedAccountId: 0
        );

        await _mediator.Send(command);
    }
}


