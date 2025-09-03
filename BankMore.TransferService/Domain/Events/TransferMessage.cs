namespace BankMore.TransferService.Domain.Events
{
    public class TransferMessage
    {
        public TransferMessage()   { }

        public string RequisitionId { get; set; }
        public int AccountId { get; set; }
    }
}
