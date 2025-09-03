namespace BankMore.TariffService.Domain.Events;

public class TransferMessage
{
    public string RequisitionId { get; set; }
    public int AccountId { get; set; }
}