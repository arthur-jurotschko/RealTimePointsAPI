-- JSON Para testar o request

--{
--    "transactionId": 1,
--    "customerId": 12345,
--    "amount": 150.00
--}

-- Criar o Banco de Dados
CREATE DATABASE SenexDB;
GO

-- Usar o Banco de Dados
USE SenexDB;
GO

-- Tabela CONSUMO
CREATE TABLE CONSUMO (
    id_transacao INT IDENTITY PRIMARY KEY,
    id_pessoa_que_consumiu INT NOT NULL,
    valor_total DECIMAL(10, 2) NOT NULL,
    pontos_obtidos_na_transacao INT NOT NULL DEFAULT 0,
    ProcessadoTempoReal BIT NOT NULL DEFAULT 0
);

-- Tabela MEMORIAL
CREATE TABLE MEMORIAL (
    id_memorial INT IDENTITY PRIMARY KEY,
    id_transacao INT NOT NULL,
    pontos_calculados INT NOT NULL,
    data_processo DATETIME NOT NULL DEFAULT GETDATE()
);

-- Tabela PONTOS
CREATE TABLE PONTOS (
    id_pessoa_que_consumiu INT PRIMARY KEY,
    saldo_de_pontos INT NOT NULL DEFAULT 0,
    data_atualizacao DATETIME NOT NULL DEFAULT GETDATE()
);

-- Verificar a tabela CONSUMO
SELECT * FROM CONSUMO;

-- Verificar a tabela PONTOS
SELECT * FROM PONTOS;

-- Verificar a tabela MEMORIAL
SELECT * FROM MEMORIAL;

-- Consultar dados combinados de PONTOS e CONSUMO
SELECT 
    P.id_pessoa_que_consumiu,
    P.saldo_de_pontos,
    P.data_atualizacao AS ultima_atualizacao,
    C.id_transacao,
    C.valor_total,
    C.pontos_obtidos_na_transacao,
    C.ProcessadoTempoReal
FROM PONTOS P
LEFT JOIN CONSUMO C ON P.id_pessoa_que_consumiu = C.id_pessoa_que_consumiu
WHERE P.id_pessoa_que_consumiu = 12345;

-- Limpar o saldo de pontos
UPDATE PONTOS
SET saldo_de_pontos = 0, data_atualizacao = GETDATE()
WHERE id_pessoa_que_consumiu = 12345;

-- Apagar todos os dados da tabela CONSUMO
DELETE FROM CONSUMO;

-- Apagar todos os dados da tabela PONTOS
DELETE FROM PONTOS;

--Apagar todos os dados da tabela MEMORIAL
DELETE FROM MEMORIAL;

-- Resetar IDENTITY da tabela CONSUMO
DBCC CHECKIDENT ('CONSUMO', RESEED, 0);

-- Resetar IDENTITY da tabela MEMORIAL
DBCC CHECKIDENT ('MEMORIAL', RESEED, 0);

-- Se PONTOS tiver coluna IDENTITY, resetar também
DBCC CHECKIDENT ('PONTOS', RESEED, 0);


