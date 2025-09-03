using MediatR;

namespace BankMore.AccountService.Application.Commands
{
    public class CreateIdempotencyKeyCommand : IRequest
    {
        public string Key { get; set; }
        public string Request { get; set; }
        public string Result { get; set; }
    }
}
