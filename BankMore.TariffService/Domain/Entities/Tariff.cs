namespace BankMore.TariffService.Domain.Entities;

public class Tariff
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public decimal Value { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
}