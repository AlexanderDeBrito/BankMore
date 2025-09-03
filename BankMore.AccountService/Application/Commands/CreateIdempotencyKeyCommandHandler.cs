using BankMore.AccountService.Domain.Entities;
using BankMore.AccountService.Domain.Interfaces;
using MediatR;

namespace BankMore.AccountService.Application.Commands
{
    public class CreateIdempotencyKeyCommandHandler : IRequestHandler<CreateIdempotencyKeyCommand>
    {
        private readonly IIdempotencyRepository _idempotencyRepository;

        public CreateIdempotencyKeyCommandHandler(IIdempotencyRepository idempotencyRepository)
        {
            _idempotencyRepository = idempotencyRepository;
        }

        public async Task Handle(CreateIdempotencyKeyCommand request, CancellationToken cancellationToken)
        {
            var idem = new IdempotencyKey(request.Key, request.Request, request.Result);
            await _idempotencyRepository.SaveAsync(idem);
        }
    }
}
