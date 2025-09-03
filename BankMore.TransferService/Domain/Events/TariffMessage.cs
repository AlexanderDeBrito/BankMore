namespace BankMore.TransferService.Domain.Events
{
    public class TariffMessage
    {
        public TariffMessage()
        {
        }

        public int AccountId { get; set; }
        public decimal Valor { get; set; }
    }
}
