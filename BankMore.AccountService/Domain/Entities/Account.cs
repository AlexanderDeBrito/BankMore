namespace BankMore.AccountService.Domain.Entities;

public class Account
{
    public int Id { get; private set; }
    public string Number { get; private set; }
    public string Name { get; private set; }
    public bool Active { get; private set; } = true;
    public string PasswordHash { get; private set; }
    public string Salt { get; private set; }
    public string Cpf { get; private set; }

    public Account() { }

    public Account(string number, string name, string password)
    {
        Number = number;
        Name = name;
        SetPassword(password);
    }

    public Account(int id, string number, string name, bool active, string passwordHash, string salt)
    {
        Id = id;
        Number = number;
        Name = name;
        Active = active;
        PasswordHash = passwordHash;
        Salt = salt;
    }

    private void SetPassword(string password)
    {
        Salt = BCrypt.Net.BCrypt.GenerateSalt();
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, Salt);
    }

    public bool ValidatePassword(string password) => BCrypt.Net.BCrypt.Verify(password, PasswordHash);

    public void Inactivate() => Active = false;

    public void Activate() => Active = true;
}