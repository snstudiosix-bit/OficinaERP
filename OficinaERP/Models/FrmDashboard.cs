using Microsoft.Data.Sqlite;
using OficinaERP.Database;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OficinaERP.Forms
{
    public class FrmDashboard : Form
    {
        public FrmDashboard()
        {
            Text = "Dashboard - Visão Geral";
            Width = 860;
            Height = 620;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(240, 240, 245);

            var lblTitulo = new Label()
            {
                Text = "📊 Dashboard",
                Left = 20,
                Top = 15,
                Width = 400,
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 80)
            };

            var lblMes = new Label()
            {
                Text = $"Mês atual: {DateTime.Now:MMMM/yyyy}",
                Left = 20,
                Top = 50,
                Width = 300,
                Font = new Font("Arial", 9),
                ForeColor = Color.Gray
            };

            // Cards de resumo
            var cardOS = CriarCard("🔧 OS Abertas", ObterValor("SELECT COUNT(*) FROM OrdensServico WHERE Status != 'Entregue'"), Color.FromArgb(70, 130, 180), 20, 90);
            var cardFaturamento = CriarCard("💰 Faturamento Mês", ObterFaturamentoMes(), Color.FromArgb(60, 160, 80), 220, 90);
            var cardReceber = CriarCard("📥 A Receber", ObterValorFinanceiro("Receber"), Color.FromArgb(200, 140, 30), 420, 90);
            var cardPagar = CriarCard("📤 A Pagar", ObterValorFinanceiro("Pagar"), Color.FromArgb(190, 60, 60), 620, 90);

            // Tabela OS recentes
            var lblOS = new Label()
            {
                Text = "Últimas Ordens de Serviço",
                Left = 20,
                Top = 210,
                Width = 300,
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 80)
            };

            var gridOS = new DataGridView()
            {
                Left = 20,
                Top = 235,
                Width = 390,
                Height = 160,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            gridOS.Columns.Add("OS", "OS#");
            gridOS.Columns.Add("Cliente", "Cliente");
            gridOS.Columns.Add("Status", "Status");
            gridOS.Columns.Add("Valor", "Valor");

            using (var conn = Conexao.Abrir())
            {
                string sql = @"SELECT os.Id, c.Nome, os.Status, os.ValorTotal
                               FROM OrdensServico os
                               INNER JOIN Clientes c ON c.Id = os.ClienteId
                               ORDER BY os.Id DESC LIMIT 6";
                using var cmd = new SqliteCommand(sql, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int i = gridOS.Rows.Add(
                        reader["Id"].ToString(),
                        reader["Nome"].ToString(),
                        reader["Status"].ToString(),
                        $"R$ {Convert.ToDouble(reader["ValorTotal"]):F2}"
                    );
                    string status = reader["Status"].ToString();
                    if (status == "Aberta") gridOS.Rows[i].DefaultCellStyle.BackColor = Color.LightYellow;
                    else if (status == "Concluída") gridOS.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                    else if (status == "Em andamento") gridOS.Rows[i].DefaultCellStyle.BackColor = Color.LightBlue;
                }
            }

            // Tabela estoque baixo
            var lblEstoque = new Label()
            {
                Text = "⚠️ Peças com Estoque Baixo",
                Left = 430,
                Top = 210,
                Width = 300,
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.DarkRed
            };

            var gridEstoque = new DataGridView()
            {
                Left = 430,
                Top = 235,
                Width = 390,
                Height = 160,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            gridEstoque.Columns.Add("Nome", "Peça");
            gridEstoque.Columns.Add("Estoque", "Estoque");
            gridEstoque.Columns.Add("Minimo", "Mínimo");

            using (var conn = Conexao.Abrir())
            {
                string sql = "SELECT Nome, QuantidadeEstoque, EstoqueMinimo FROM Pecas WHERE QuantidadeEstoque <= EstoqueMinimo ORDER BY QuantidadeEstoque ASC LIMIT 6";
                using var cmd = new SqliteCommand(sql, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int i = gridEstoque.Rows.Add(
                        reader["Nome"].ToString(),
                        Convert.ToDouble(reader["QuantidadeEstoque"]).ToString("F2"),
                        Convert.ToDouble(reader["EstoqueMinimo"]).ToString("F2")
                    );
                    gridEstoque.Rows[i].DefaultCellStyle.BackColor = Color.LightCoral;
                }
            }

            // Tabela financeiro pendente
            var lblFin = new Label()
            {
                Text = "📋 Lançamentos Pendentes",
                Left = 20,
                Top = 415,
                Width = 300,
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 80)
            };

            var gridFin = new DataGridView()
            {
                Left = 20,
                Top = 440,
                Width = 800,
                Height = 120,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            gridFin.Columns.Add("Tipo", "Tipo");
            gridFin.Columns.Add("Descricao", "Descrição");
            gridFin.Columns.Add("Valor", "Valor R$");
            gridFin.Columns.Add("Vencimento", "Vencimento");

            using (var conn = Conexao.Abrir())
            {
                string sql = "SELECT Tipo, Descricao, Valor, DataVencimento FROM Financeiro WHERE Status = 'Pendente' ORDER BY DataVencimento ASC LIMIT 8";
                using var cmd = new SqliteCommand(sql, conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string tipo = reader["Tipo"].ToString();
                    int i = gridFin.Rows.Add(
                        tipo,
                        reader["Descricao"].ToString(),
                        $"R$ {Convert.ToDouble(reader["Valor"]):F2}",
                        reader["DataVencimento"].ToString()
                    );
                    gridFin.Rows[i].DefaultCellStyle.BackColor = tipo == "Pagar" ? Color.LightCoral : Color.LightYellow;
                }
            }

            Controls.AddRange(new Control[] {
                lblTitulo, lblMes,
                cardOS, cardFaturamento, cardReceber, cardPagar,
                lblOS, gridOS,
                lblEstoque, gridEstoque,
                lblFin, gridFin
            });
        }

        private Panel CriarCard(string titulo, string valor, Color cor, int left, int top)
        {
            var panel = new Panel()
            {
                Left = left,
                Top = top,
                Width = 180,
                Height = 100,
                BackColor = cor,
                BorderStyle = BorderStyle.None
            };

            var lblTitulo = new Label()
            {
                Text = titulo,
                Left = 10,
                Top = 10,
                Width = 160,
                Font = new Font("Arial", 9, FontStyle.Bold),
                ForeColor = Color.White
            };

            var lblValor = new Label()
            {
                Text = valor,
                Left = 10,
                Top = 45,
                Width = 160,
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.White
            };

            panel.Controls.Add(lblTitulo);
            panel.Controls.Add(lblValor);
            return panel;
        }

        private string ObterValor(string sql)
        {
            using var conn = Conexao.Abrir();
            using var cmd = new SqliteCommand(sql, conn);
            var result = cmd.ExecuteScalar();
            return result?.ToString() ?? "0";
        }

        private string ObterFaturamentoMes()
        {
            using var conn = Conexao.Abrir();
            string mes = DateTime.Now.ToString("MM/yyyy");
            string sql = "SELECT COALESCE(SUM(ValorTotal), 0) FROM OrdensServico WHERE Status = 'Entregue' AND DataAbertura LIKE @mes";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@mes", $"%{mes}%");
            double valor = Convert.ToDouble(cmd.ExecuteScalar());
            return $"R$ {valor:F2}";
        }

        private string ObterValorFinanceiro(string tipo)
        {
            using var conn = Conexao.Abrir();
            string sql = "SELECT COALESCE(SUM(Valor), 0) FROM Financeiro WHERE Tipo = @tipo AND Status = 'Pendente'";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@tipo", tipo);
            double valor = Convert.ToDouble(cmd.ExecuteScalar());
            return $"R$ {valor:F2}";
        }
    }
}