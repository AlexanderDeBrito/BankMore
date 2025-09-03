using MediatR;
using BankMore.AccountService.Domain.Entities;
using BankMore.AccountService.Domain.Interfaces;

namespace BankMore.AccountService.Application.Commands;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, string>
{
    private readonly IAccountRepository _repo;

    public CreateAccountCommandHandler(IAccountRepository repo) => _repo = repo;

    public async Task<string> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        // Valida CPF (simples regex)
        if (!IsValidCpf(request.Cpf)) throw new InvalidOperationException("INVALID_DOCUMENT: CPF inválido.");

        var account = new Account(request.Cpf, request.Name, request.Password);
        await _repo.CreateAsync(account);
        return account.Number;
    }

    private bool IsValidCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        // Remove caracteres não numéricos
        cpf = new string(cpf.Where(char.IsDigit).ToArray());

        if (cpf.Length != 11)
            return false;

        // Evita CPFs com todos os números iguais
        if (cpf.All(c => c == cpf[0]))
            return false;

        // Calcula o primeiro dígito verificador
        int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCpf = cpf.Substring(0, 9);
        int soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        int resto = soma % 11;
        int digito1 = resto < 2 ? 0 : 11 - resto;

        tempCpf += digito1;
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        int digito2 = resto < 2 ? 0 : 11 - resto;

        string digitosCalculados = $"{digito1}{digito2}";
        string digitosInformados = cpf.Substring(9, 2);

        return digitosCalculados == digitosInformados;
    }
}