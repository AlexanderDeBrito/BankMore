namespace BankMore.AccountService.Domain.Entities;

public class Movement
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string Type { get; set; } // "C" or "D"
    public decimal Value { get; set; }

    public Movement(int accountId, DateTime date, string type, decimal value)
    {
        AccountId = accountId;
        Date = date;
        Type = type;
        Value = value;
    }

    public void Validate(bool isOwnAccount)
    {
        if (Value <= 0) throw new InvalidOperationException("INVALID_VALUE: Apenas valores positivos podem ser recebidos.");
        if (Type != "C" && Type != "D") throw new InvalidOperationException("INVALID_TYPE: Apenas os tipos “débito” ou “crédito” podem ser aceitos.");
        if (!isOwnAccount && Type != "C") throw new InvalidOperationException("INVALID_TYPE: Apenas o tipo “crédito” pode ser aceito caso o número da conta seja diferente do usuário logado.");
    }
}