namespace BankMore.TransferService.Domain.Entities;

public class Transfer
{
    public int Id { get; set; }
    public int OriginAccountId { get; set; }
    public string DestinationAccountNumber { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public decimal Value { get; set; }

    public void Validate(int originAccountId)
    {
        if (Value <= 0) throw new InvalidOperationException("INVALID_VALUE: Apenas valores positivos podem ser recebidos.");
        
    }
}