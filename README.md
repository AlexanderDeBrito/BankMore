# BankMore - Sistema Bancário Distribuído

Bem-vindo ao projeto **BankMore**, um sistema bancário distribuído desenvolvido em .NET 8 que simula operações de conta, transferência e tarifação usando microserviços e Apache Kafka. Este repositório contém três serviços principais:

- **BankMore.AccountService**: Gerencia contas bancárias e movimentos financeiros.
- **BankMore.TransferService**: Processa transferências entre contas.
- **BankMore.TariffService**: Aplica tarifas às transferências processadas.

## Pré-requisitos

Antes de rodar o projeto, certifique-se de ter as seguintes ferramentas instaladas:

- **.NET 8 SDK**: [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Docker**: [Download](https://www.docker.com/products/docker-desktop) (inclui Docker Compose)
- **Git**: [Download](https://git-scm.com/downloads) (para clonar o repositório)

### Requisitos Opcionais
- Um visualizador SQLite (ex.: DB Browser for SQLite) para inspecionar os bancos de dados (`account.db`, `transfer.db`, `tariff.db`).
- Um cliente Kafka (ex.: `kafka-console-consumer`) para monitoramento (disponível via Docker).

## Estrutura do Projeto

- `src/BankMore.AccountService/`: Serviço de contas bancárias (porta 5001).
- `src/BankMore.TransferService/`: Serviço de transferências (porta 5002).
- `src/BankMore.TariffService/`: Worker de tarifação (sem porta fixa)-pelo docker roda em (5003).
- `docker-compose.yaml`: Configuração para rodar todos os serviços e dependências.

## Configuração e Execução

### Passo 1: Clone o Repositório
```bash
git clone https://github.com/seu-usuario/bankmore.git
cd bankmore
