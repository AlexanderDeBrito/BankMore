using BankMore.TransferService.Domain.Interfaces;
using BankMore.TransferService.Infrastructure.Cache;
using BankMore.TransferService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using System.Text;
using KafkaFlow;
using BankMore.TransferService.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Transfer API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, new string[] { } }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
        };
    });


//Kafka
builder.Services.AddKafka(kafka => kafka  
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { builder.Configuration["Kafka:BootstrapServers"] })
        .AddProducer("transfer-producer", producer => producer
            .DefaultTopic(builder.Configuration["Kafka:TransferTopic"])
        )
    )
);



builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));


// HttpClient para chamadas ao AccountService com Polly
builder.Services.AddHttpClient("AccountClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AccountServiceUrl"]);
})
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError() // Trata  408, timeouts, etc.
    .Or<HttpRequestException>() 
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

// Redis -- Serviço de cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("Redis:ConnectionString");
    options.InstanceName = "BankMoreCache_";
});

// Registra o HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Registra repositories
builder.Services.AddScoped<ITransferRepository, TransferRepository>();
builder.Services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();
builder.Services.AddScoped<CacheService>();
builder.Services.AddScoped<TransferProducer>();

//// Força a porta 5001 (ajuste para desenvolvimento)
//builder.WebHost.UseUrls("http://localhost:5001");

var app = builder.Build();

// Inicializa o DB
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        DbInitializer.Initialize(services.GetRequiredService<IConfiguration>());
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao inicializar o banco de dados: {ex.Message}");
        throw;
    }
}

// Habilita Swagger apenas em ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Transfer API v1");
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();