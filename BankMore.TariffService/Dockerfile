# Estágio de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["BankMore.TariffService.csproj", "."]
RUN dotnet restore "BankMore.TariffService.csproj"

COPY . .
RUN dotnet publish "BankMore.TariffService.csproj" -c Release -o /app/publish

# Estágio final com runtime ASP.NET
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV DOTNET_ENVIRONMENT=Development

ENTRYPOINT ["dotnet", "BankMore.TariffService.dll"]
