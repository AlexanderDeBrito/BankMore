using BankMore.TransferService.Domain.Events;

namespace BankMore.TransferService.Application.Interfaces
{
    public interface ITransferProducer
    {
        Task ProduceAsync(TransferMessage message);
    }
}
