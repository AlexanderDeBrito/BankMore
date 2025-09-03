using MediatR;
using BankMore.AccountService.Domain.Entities;
using BankMore.AccountService.Domain.Interfaces;

namespace BankMore.AccountService.Application.Commands;

public class CreateMovementCommandHandler : IRequestHandler<CreateMovementCommand>
{
    private readonly IMovementRepository _movementRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IIdempotencyRepository _idempotencyRepository;

    public CreateMovementCommandHandler(IMovementRepository movementRepository, IAccountRepository accountRepository, IIdempotencyRepository idempotencyRepository)
    {
        _movementRepository = movementRepository;
        _accountRepository = accountRepository;
        _idempotencyRepository = idempotencyRepository;
    }

    public async Task Handle(CreateMovementCommand request, CancellationToken cancellationToken)
    {
        // Idempotência
        var existing = await _idempotencyRepository.GetByKeyAsync(request.IdempotencyKey);
        if (existing != null && existing.Result == "Success") return;

        int targetAccountId = request.LoggedAccountId;
        bool isOwnAccount = true;

        if (!string.IsNullOrEmpty(request.AccountNumber))
        {
            var targetAccount = await _accountRepository.GetByNumberAsync(request.AccountNumber);
            if (targetAccount == null) throw new InvalidOperationException("INVALID_ACCOUNT: Conta não cadastrada.");
            targetAccountId = targetAccount.Id;
            isOwnAccount = targetAccountId == request.LoggedAccountId;
        }

        var account = await _accountRepository.GetByIdAsync(targetAccountId);
        if (account == null) throw new InvalidOperationException("INVALID_ACCOUNT: Conta não cadastrada.");
        if (!account.Active) throw new InvalidOperationException("INACTIVE_ACCOUNT: Conta inativa.");

        var movement = new Movement ( targetAccountId, DateTime.Now , request.Type, request.Value);
        movement.Validate(isOwnAccount);

        await _movementRepository.CreateAsync(movement);

        // Salva idempotência
        await _idempotencyRepository.SaveAsync(new IdempotencyKey(request.IdempotencyKey, "Movement", "Success" ));
    }
}