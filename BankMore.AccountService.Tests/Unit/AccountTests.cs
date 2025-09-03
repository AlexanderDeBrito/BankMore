using BankMore.AccountService.Application.Commands;
using BankMore.AccountService.Domain.Entities;
using BankMore.AccountService.Domain.Interfaces;
using Moq;

namespace BankMore.AccountService.Tests.UnitTests;

public class AccountTests
{
    private readonly Mock<IAccountRepository> _mockRepository;
    private readonly CreateAccountCommandHandler _handler;

    public AccountTests()
    {
        _mockRepository = new Mock<IAccountRepository>();
        _handler = new CreateAccountCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesAccountSuccessfully()
    {
        // Arrange
        var command = new CreateAccountCommand
        {
            Cpf = "06237502017", // CPF válido para teste
            Password = "password123",
            Name = "João Silva"
        };
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Account>()))
                       .Returns(Task.FromResult(true));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(!string.IsNullOrEmpty(result));
        _mockRepository.Verify(r => r.CreateAsync(It.Is<Account>(c => c.Name == "João Silva")), Times.Once());
    }


    [Fact]
    public async Task Handle_InvalidCpf_ThrowsArgumentException()
    {
        // Arrange
        var command = new CreateAccountCommand
        {
            Cpf = "1111111", // CPF inválido
            Password = "password123",
            Name = "João Silva"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}