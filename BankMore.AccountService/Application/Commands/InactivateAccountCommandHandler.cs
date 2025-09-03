using MediatR;
using BankMore.AccountService.Domain.Interfaces;

namespace BankMore.AccountService.Application.Commands;

public class InactivateAccountCommandHandler : IRequestHandler<InactivateAccountCommand>
{
    private readonly IAccountRepository _repository;

    public InactivateAccountCommandHandler(IAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(InactivateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _repository.GetByIdAsync(request.AccountId);
        if (account == null)
        {
            throw new InvalidOperationException("INVALID_ACCOUNT: Conta não cadastrada.");
        }

        if (!account.ValidatePassword(request.Password))
        {
            throw new UnauthorizedAccessException("USER_UNAUTHORIZED: Senha inválida.");
        }

        account.Inactivate();
        await _repository.UpdateAsync(account);
    }
}