using BankMore.TransferService.Application.Commands;
using BankMore.TransferService.Application.Interfaces;
using BankMore.TransferService.Domain.Entities;
using BankMore.TransferService.Domain.Events;
using BankMore.TransferService.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.Protected;
using System.Net;

namespace BankMore.TransferService.Tests.UnitTests;

public class PerformTransferCommandHandlerTests
{
    private readonly Mock<ITransferRepository> _mockTransferRepository;
    private readonly Mock<IIdempotencyRepository> _mockIdempotencyRepository;
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<ITransferProducer> _mockProducer;
    private readonly PerformTransferCommandHandler _handler;

    public PerformTransferCommandHandlerTests()
    {
        _mockTransferRepository = new Mock<ITransferRepository>();
        _mockIdempotencyRepository = new Mock<IIdempotencyRepository>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockProducer = new Mock<ITransferProducer>();
        _handler = new PerformTransferCommandHandler(
            _mockTransferRepository.Object,
            _mockIdempotencyRepository.Object,
            _mockHttpClientFactory.Object,
            _mockHttpContextAccessor.Object,
            _mockProducer.Object);

    }

    [Fact]
    public async Task Handle_ValidTransfer_SucceedsAndProducesMessage()
    {
       
        // Mock do HttpContextAccessor com Authorization Header
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = "Bearer valid-token";
        _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(httpContext);

        // Comando de teste
        var command = new PerformTransferCommand
        {
            IdempotencyKey = "unique-id-123",
            DestinationAccountNumber = "98765432109",
            Value = 100,
            LoggedAccountId = 1
        };

        // Mocks dos repositórios e producer
        _mockIdempotencyRepository.Setup(r => r.GetByKeyAsync(It.IsAny<string>()))
            .ReturnsAsync((IdempotencyKey)null); // Simula que não existe ainda

        _mockTransferRepository.Setup(r => r.CreateAsync(It.IsAny<Transfer>()))
            .Returns(Task.CompletedTask);

        _mockProducer.Setup(p => p.ProduceAsync(It.IsAny<TransferMessage>()))
            .Returns(Task.CompletedTask);

        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"id\":1,\"balance\":1000}")
            });

        var client = new HttpClient(mockHandler.Object);

        // setup do IHttpClientFactory para retornar este client
        _mockHttpClientFactory
            .Setup(f => f.CreateClient("AccountClient"))
            .Returns(client);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockTransferRepository.Verify(r => r.CreateAsync(It.IsAny<Transfer>()), Times.Once());
        _mockProducer.Verify(p => p.ProduceAsync(It.IsAny<TransferMessage>()), Times.Once());
    }
}