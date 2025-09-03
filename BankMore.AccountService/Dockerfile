FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BankMore.AccountService.csproj", "."]
RUN dotnet restore "BankMore.AccountService.csproj"
COPY . .
RUN dotnet build "BankMore.AccountService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BankMore.AccountService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankMore.AccountService.dll"]