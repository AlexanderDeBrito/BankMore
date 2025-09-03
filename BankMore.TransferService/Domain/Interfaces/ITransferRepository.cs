using BankMore.TransferService.Domain.Entities;

namespace BankMore.TransferService.Domain.Interfaces;

public interface ITransferRepository
{
    Task CreateAsync(Transfer transfer);
}