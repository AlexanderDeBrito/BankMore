using BankMore.AccountService.Domain.Interfaces;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankMore.AccountService.Application.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
{
    private readonly IAccountRepository _repository;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(IAccountRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var account = await _repository.GetByNumberAsync(request.Identifier);
        if (account == null || !account.ValidatePassword(request.Password))
        {
            throw new UnauthorizedAccessException("USER_UNAUTHORIZED: Credenciais inválidas.");
        }

        // Gera JWT
        var claims = new[]
        {
            new Claim("accountId", account.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:ExpiryMinutes"])),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // Retorna já com "Bearer" na frente pra facilitar o uso no swagger.
        return $"Bearer {tokenString}";
    }
}