using BankMore.TransferService.Application.Interfaces;
using BankMore.TransferService.Domain.Entities;
using BankMore.TransferService.Domain.Events;
using BankMore.TransferService.Domain.Interfaces;
using BankMore.TransferService.Infrastructure.Messaging;
using MediatR;
using System.Text.Json;

namespace BankMore.TransferService.Application.Commands;

public class PerformTransferCommandHandler : IRequestHandler<PerformTransferCommand>
{
    private readonly ITransferRepository _transferRepository;
    private readonly IIdempotencyRepository _idempotencyRepository;
    private readonly HttpClient _accountClient;
    private readonly IHttpContextAccessor _httpContextAccessor;    
    private readonly ITransferProducer _producer;

    public PerformTransferCommandHandler(ITransferRepository transferRepository, IIdempotencyRepository idempotencyRepository, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ITransferProducer producer)
    {
        _transferRepository = transferRepository;
        _idempotencyRepository = idempotencyRepository;
        _accountClient = httpClientFactory.CreateClient("AccountClient") ?? new HttpClient(new FakeHttpMessageHandler()) { BaseAddress = new Uri("http://localhost") };
        _httpContextAccessor = httpContextAccessor;
        _producer = producer;
    }

    public async Task Handle(PerformTransferCommand request, CancellationToken cancellationToken)
    {

        var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            throw new UnauthorizedAccessException("Token de autenticação ausente ou inválido.");
        }

        var token = authorizationHeader.Replace("Bearer ", "");
        _accountClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var existing = await _idempotencyRepository.GetByKeyAsync(request.IdempotencyKey);
        if (existing != null && existing.Result == "Success") return;

        var transfer = new Transfer { OriginAccountId = request.LoggedAccountId, DestinationAccountNumber = request.DestinationAccountNumber, Value = request.Value };
        transfer.Validate(request.LoggedAccountId);

        try
        {
            var debitPayload = new { IdempotencyKey = request.IdempotencyKey, Value = request.Value, Type = "D" };
            var debitResponse = await _accountClient.PostAsJsonAsync("/movements", debitPayload, cancellationToken);
            if (!debitResponse.IsSuccessStatusCode)
            {
                var error = await debitResponse.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Falha no débito: {error}");
            }

            try
            {
                var creditPayload = new { IdempotencyKey = request.IdempotencyKey, AccountNumber = request.DestinationAccountNumber, Value = request.Value, Type = "C" };
                var creditResponse = await _accountClient.PostAsJsonAsync("/movements", creditPayload, cancellationToken);
                if (!creditResponse.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException("Falha no crédito");
                }

                await _transferRepository.CreateAsync(transfer);

                // Produz mensagem para o tópico de transferências realizadas
                await _producer.ProduceAsync(new TransferMessage { RequisitionId = request.IdempotencyKey, AccountId = request.LoggedAccountId });

                await _idempotencyRepository.SaveAsync(new IdempotencyKey { Key = request.IdempotencyKey, Request = JsonSerializer.Serialize(request), Result = "Success" });
            }
            catch
            {
                var estornoPayload = new { IdempotencyKey = Guid.NewGuid().ToString(), Value = request.Value, Type = "C" };
                await _accountClient.PostAsJsonAsync("/movements", estornoPayload, cancellationToken);
                await _idempotencyRepository.SaveAsync(new IdempotencyKey { Key = request.IdempotencyKey, Request = JsonSerializer.Serialize(request), Result = "Failure" });
                throw new InvalidOperationException("Falha no crédito, estorno realizado.");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Falha na transferência: {ex.Message}");
        }
    }
}