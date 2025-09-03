namespace BankMore.AccountService.Domain.Events
{
    public class TariffMessage
    {
        public int AccountNumber { get; set; }
        public decimal Valor { get; set; }
    }
}
