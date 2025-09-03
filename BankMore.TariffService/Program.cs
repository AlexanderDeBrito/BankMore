using BankMore.TariffService.Infrastructure.Messaging;
using BankMore.TariffService.Infrastructure.Persistence;
using KafkaFlow;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Configurações
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Registrando serviços
builder.Services.AddScoped<TariffRepository>();
builder.Services.AddScoped<TariffProducer>();

// KafkaFlow
builder.Services.AddKafka(kafka => kafka
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { builder.Configuration["Kafka:BootstrapServers"] })
        .AddConsumer(consumer => consumer
            .Topic(builder.Configuration["Kafka:TransferTopic"])
            .WithGroupId(builder.Configuration["Kafka:GroupId"])
            .WithBufferSize(100)
            .WithWorkersCount(10)
            .AddMiddlewares(middlewares => middlewares
                .AddTypedHandlers(h => h
                    .WithHandlerLifetime(InstanceLifetime.Scoped)  // Scoped para evitar erro de ciclo de vida
                    .AddHandler<TransferConsumer>()
                )
            )
        )
        .AddProducer("tariff-producer", producer => producer
            .DefaultTopic(builder.Configuration["Kafka:TariffTopic"])
        )
    )
);

var host = builder.Build();

// Inicializa o banco de dados
DbInitializer.Initialize(builder.Configuration);

await host.RunAsync();