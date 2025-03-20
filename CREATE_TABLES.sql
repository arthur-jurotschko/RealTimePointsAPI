
CREATE TABLE PONTOS (
    id_pessoa_que_consumiu INT PRIMARY KEY,
    saldo_de_pontos INT NOT NULL DEFAULT 0,
    data_atualizacao DATETIME NOT NULL DEFAULT GETDATE()
);

---------------------------------------------------------------------------

CREATE TABLE CONSUMO (
    id_transacao INT IDENTITY PRIMARY KEY,
    id_pessoa_que_consumiu INT NOT NULL,
    valor_total DECIMAL(10, 2) NOT NULL,
    pontos_obtidos_na_transacao INT NOT NULL,
    ProcessadoTempoReal BIT NOT NULL DEFAULT 0
);