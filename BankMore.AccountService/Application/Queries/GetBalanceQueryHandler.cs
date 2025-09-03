using BankMore.AccountService.Domain.Interfaces;
using BankMore.AccountService.Infrastructure.Cache;
using MediatR;

namespace BankMore.AccountService.Application.Queries;

public class GetBalanceQueryHandler : IRequestHandler<GetBalanceQuery, BalanceResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMovementRepository _movementRepository;
    private readonly CacheService _cacheService;

    public GetBalanceQueryHandler(IAccountRepository accountRepository, IMovementRepository movementRepository, CacheService cacheService)
    {
        _accountRepository = accountRepository;
        _movementRepository = movementRepository;
        _cacheService = cacheService;
    }

    public async Task<BalanceResponse> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"balance_{request.AccountId}";
        var cached = await _cacheService.GetAsync<BalanceResponse>(cacheKey);
        if (cached != null) return cached;

        var account = await _accountRepository.GetByIdAsync(request.AccountId);
        if (account == null) throw new InvalidOperationException("INVALID_ACCOUNT: Conta não cadastrada.");
        if (!account.Active) throw new InvalidOperationException("INACTIVE_ACCOUNT: Conta inativa.");

        var credits = await _movementRepository.GetSumByTypeAsync(request.AccountId, "C");
        var debits = await _movementRepository.GetSumByTypeAsync(request.AccountId, "D");
        var balance = credits - debits;

        var response = new BalanceResponse
        {
            AccountNumber = account.Number,
            Name = account.Name,
            QueryDate = DateTime.UtcNow,
            Balance = balance >= 0 ? balance : 0m
        };

        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(1)); // Cache por 1 minuto
        return response;
    }
}