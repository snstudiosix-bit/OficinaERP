using Microsoft.Data.Sqlite;

namespace OficinaERP.Database
{
    public class Banco
    {
        public static void CriarBanco()
        {
            using var conn = Conexao.Abrir();

            string sql = @"
                CREATE TABLE IF NOT EXISTS Clientes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nome TEXT NOT NULL,
                    Telefone TEXT,
                    CpfCnpj TEXT,
                    Endereco TEXT
                );

                CREATE TABLE IF NOT EXISTS Veiculos (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClienteId INTEGER NOT NULL,
                    Placa TEXT NOT NULL,
                    Marca TEXT,
                    Modelo TEXT,
                    Ano TEXT,
                    Cor TEXT,
                    Quilometragem TEXT,
                    FOREIGN KEY (ClienteId) REFERENCES Clientes(Id)
                );

                CREATE TABLE IF NOT EXISTS OrdensServico (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClienteId INTEGER NOT NULL,
                    VeiculoId INTEGER NOT NULL,
                    DataAbertura TEXT NOT NULL,
                    DefeitoInformado TEXT,
                    Diagnostico TEXT,
                    ServicosExecutados TEXT,
                    PecasUtilizadas TEXT,
                    ValorMaoDeObra REAL,
                    ValorTotal REAL,
                    Status TEXT NOT NULL DEFAULT 'Aberta',
                    Observacoes TEXT,
                    FOREIGN KEY (ClienteId) REFERENCES Clientes(Id),
                    FOREIGN KEY (VeiculoId) REFERENCES Veiculos(Id)
                );

                CREATE TABLE IF NOT EXISTS Pecas (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nome TEXT NOT NULL,
                    Codigo TEXT,
                    Descricao TEXT,
                    QuantidadeEstoque REAL DEFAULT 0,
                    EstoqueMinimo REAL DEFAULT 0,
                    ValorCusto REAL DEFAULT 0,
                    ValorVenda REAL DEFAULT 0
                );

                CREATE TABLE IF NOT EXISTS MovimentacaoEstoque (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PecaId INTEGER NOT NULL,
                    Tipo TEXT NOT NULL,
                    Quantidade REAL NOT NULL,
                    Data TEXT NOT NULL,
                    Observacao TEXT,
                    FOREIGN KEY (PecaId) REFERENCES Pecas(Id)
                );

                CREATE TABLE IF NOT EXISTS Financeiro (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Tipo TEXT NOT NULL,
                    Descricao TEXT NOT NULL,
                    Valor REAL NOT NULL,
                    DataVencimento TEXT NOT NULL,
                    DataPagamento TEXT,
                    Status TEXT NOT NULL DEFAULT 'Pendente',
                    Observacao TEXT
                );

                CREATE TABLE IF NOT EXISTS Vendas (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Data TEXT,
                    ClienteId INTEGER,
                    OSId INTEGER,
                    FormaPagamento TEXT,
                    Desconto REAL DEFAULT 0,
                    Total REAL,
                    Observacoes TEXT
                );

                CREATE TABLE IF NOT EXISTS VendaItens (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    VendaId INTEGER,
                    Descricao TEXT,
                    Quantidade REAL,
                    ValorUnitario REAL,
                    ValorTotal REAL,
                    Tipo TEXT
                );

                CREATE TABLE IF NOT EXISTS Usuarios (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nome TEXT NOT NULL,
                    Login TEXT NOT NULL UNIQUE,
                    Senha TEXT NOT NULL,
                    Perfil TEXT NOT NULL DEFAULT 'Operador'
                );

                INSERT OR IGNORE INTO Usuarios (Nome, Login, Senha, Perfil)
                VALUES ('Administrador', 'admin', '21232f297a57a5a743894a0e4a801fc3', 'Admin');
            ";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }
    }
}