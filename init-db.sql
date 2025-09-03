-- Script de Inicialização de Bancos de Dados para os Serviços BankMore
-- Este script cria as tabelas necessárias para os bancos de dados separados:
-- - account.db para BankMore.AccountService
-- - transfer.db para BankMore.TransferService
-- - tariff.db para BankMore.TariffService
-- Execute este script em cada banco de dados correspondente usando uma ferramenta como sqlite3 ou DB Browser for SQLite.
-- Para uso com Docker Compose, você pode montar volumes e executar o script manualmente ou via um entrypoint no Dockerfile.

-- Para account.db (BankMore.AccountService)
CREATE TABLE IF NOT EXISTS contacorrente (
    idcontacorrente INTEGER PRIMARY KEY AUTOINCREMENT,
    numeiro TEXT NOT NULL UNIQUE,  -- CPF como número da conta
    nome TEXT NOT NULL,
    ativo INTEGER NOT NULL DEFAULT 1,
    senha TEXT NOT NULL,
    salt TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS movimento (
    idmovimento INTEGER PRIMARY KEY AUTOINCREMENT,
    idcontacorrente INTEGER NOT NULL,
    datamovimento DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    tipomovimento TEXT NOT NULL CHECK(tipomovimento IN ('C', 'D')),
    valor DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (idcontacorrente) REFERENCES contacorrente(idcontacorrente)
);

CREATE TABLE IF NOT EXISTS idempotencia (
    chave_idempotencia TEXT PRIMARY KEY,
    requisicao TEXT NOT NULL,
    resultado TEXT NOT NULL
);

-- Para transfer.db (BankMore.TransferService)
CREATE TABLE IF NOT EXISTS transferencia (
    idtransferencia INTEGER PRIMARY KEY AUTOINCREMENT,
    idcontacorrente_origem INTEGER NOT NULL,
    idcontacorrente_destino TEXT NOT NULL,  -- Como string, pois destino é number
    datamovimento DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    valor DECIMAL(18,2) NOT NULL
);

CREATE TABLE IF NOT EXISTS idempotencia (
    chave_idempotencia TEXT PRIMARY KEY,
    requisicao TEXT NOT NULL,
    resultado TEXT NOT NULL
);

-- Para tariff.db (BankMore.TariffService)
CREATE TABLE IF NOT EXISTS tarifas (
    idtarifa INTEGER PRIMARY KEY AUTOINCREMENT,
    idcontacorrente INTEGER NOT NULL,
    datamovimento DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    valor DECIMAL(18,2) NOT NULL
);