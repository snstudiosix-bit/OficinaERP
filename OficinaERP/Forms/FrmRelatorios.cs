using Microsoft.Data.Sqlite;
using OficinaERP.Database;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OficinaERP.Forms
{
    public class FrmRelatorios : Form
    {
        private DateTimePicker dtpInicio = new DateTimePicker();
        private DateTimePicker dtpFim = new DateTimePicker();
        private ComboBox cmbRelatorio = new ComboBox();
        private Button btnGerar = new Button();
        private DataGridView grid = new DataGridView();
        private Label lblTotal = new Label();

        public FrmRelatorios()
        {
            Text = "Relatórios";
            Width = 860;
            Height = 580;
            StartPosition = FormStartPosition.CenterScreen;

            var lblTitulo = new Label()
            {
                Text = "📋 Relatórios",
                Left = 20,
                Top = 15,
                Width = 300,
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 80)
            };

            var lblTipo = new Label() { Text = "Relatório:", Left = 20, Top = 60, Width = 90 };
            cmbRelatorio.Left = 115; cmbRelatorio.Top = 58; cmbRelatorio.Width = 220;
            cmbRelatorio.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRelatorio.Items.AddRange(new string[] {
                "OS por período",
                "Faturamento por período",
                "Clientes cadastrados",
                "Estoque atual",
                "Peças com estoque baixo",
                "Lançamentos financeiros"
            });
            cmbRelatorio.SelectedIndex = 0;

            var lblInicio = new Label() { Text = "De:", Left = 360, Top = 60, Width = 30 };
            dtpInicio.Left = 395; dtpInicio.Top = 58; dtpInicio.Width = 140;
            dtpInicio.Format = DateTimePickerFormat.Short;
            dtpInicio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var lblFim = new Label() { Text = "Até:", Left = 545, Top = 60, Width = 35 };
            dtpFim.Left = 585; dtpFim.Top = 58; dtpFim.Width = 140;
            dtpFim.Format = DateTimePickerFormat.Short;
            dtpFim.Value = DateTime.Now;

            btnGerar.Text = "Gerar Relatório";
            btnGerar.Left = 740; btnGerar.Top = 55; btnGerar.Width = 90;
            btnGerar.Height = 28;
            btnGerar.BackColor = Color.FromArgb(30, 30, 80);
            btnGerar.ForeColor = Color.White;
            btnGerar.Click += BtnGerar_Click;

            lblTotal.Left = 20; lblTotal.Top = 100; lblTotal.Width = 500;
            lblTotal.Font = new Font("Arial", 10, FontStyle.Bold);
            lblTotal.ForeColor = Color.DarkGreen;

            grid.Left = 20; grid.Top = 130;
            grid.Width = 800; grid.Height = 390;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = Color.White;

            Controls.AddRange(new Control[] {
                lblTitulo,
                lblTipo, cmbRelatorio,
                lblInicio, dtpInicio,
                lblFim, dtpFim,
                btnGerar, lblTotal, grid
            });
        }

        private void BtnGerar_Click(object sender, EventArgs e)
        {
            grid.Rows.Clear();
            grid.Columns.Clear();
            lblTotal.Text = "";

            string inicio = dtpInicio.Value.ToString("yyyy-MM-dd");
            string fim = dtpFim.Value.ToString("yyyy-MM-dd");

            switch (cmbRelatorio.SelectedIndex)
            {
                case 0: RelatorioOS(inicio, fim); break;
                case 1: RelatorioFaturamento(inicio, fim); break;
                case 2: RelatorioClientes(); break;
                case 3: RelatorioEstoque(); break;
                case 4: RelatorioEstoqueBaixo(); break;
                case 5: RelatorioFinanceiro(inicio, fim); break;
            }
        }

        private void RelatorioOS(string inicio, string fim)
        {
            grid.Columns.Add("Id", "OS#");
            grid.Columns.Add("Data", "Data");
            grid.Columns.Add("Cliente", "Cliente");
            grid.Columns.Add("Veiculo", "Veículo");
            grid.Columns.Add("Status", "Status");
            grid.Columns.Add("Valor", "Valor R$");

            double total = 0;
            using var conn = Conexao.Abrir();
            string sql = @"SELECT os.Id, os.DataAbertura, c.Nome, v.Placa, os.Status, os.ValorTotal
                           FROM OrdensServico os
                           INNER JOIN Clientes c ON c.Id = os.ClienteId
                           INNER JOIN Veiculos v ON v.Id = os.VeiculoId
                           WHERE DATE(os.DataAbertura) BETWEEN @ini AND @fim
                           ORDER BY os.Id DESC";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ini", inicio);
            cmd.Parameters.AddWithValue("@fim", fim);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                double valor = Convert.ToDouble(reader["ValorTotal"]);
                total += valor;
                grid.Rows.Add(
                    reader["Id"].ToString(),
                    reader["DataAbertura"].ToString(),
                    reader["Nome"].ToString(),
                    reader["Placa"].ToString(),
                    reader["Status"].ToString(),
                    $"R$ {valor:F2}"
                );
            }
            lblTotal.Text = $"Total de OS: {grid.Rows.Count} | Valor total: R$ {total:F2}";
        }

        private void RelatorioFaturamento(string inicio, string fim)
        {
            grid.Columns.Add("Mes", "Mês");
            grid.Columns.Add("QtdOS", "Qtd OS");
            grid.Columns.Add("Faturamento", "Faturamento R$");

            using var conn = Conexao.Abrir();
            string sql = @"SELECT strftime('%m/%Y', DataAbertura) as Mes,
                           COUNT(*) as QtdOS, SUM(ValorTotal) as Total
                           FROM OrdensServico
                           WHERE Status = 'Entregue'
                           AND DATE(DataAbertura) BETWEEN @ini AND @fim
                           GROUP BY strftime('%m/%Y', DataAbertura)
                           ORDER BY DataAbertura DESC";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ini", inicio);
            cmd.Parameters.AddWithValue("@fim", fim);
            double totalGeral = 0;
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                double val = Convert.ToDouble(reader["Total"]);
                totalGeral += val;
                grid.Rows.Add(reader["Mes"].ToString(), reader["QtdOS"].ToString(), $"R$ {val:F2}");
            }
            lblTotal.Text = $"Faturamento total no período: R$ {totalGeral:F2}";
        }

        private void RelatorioClientes()
        {
            grid.Columns.Add("Nome", "Nome");
            grid.Columns.Add("Telefone", "Telefone");
            grid.Columns.Add("CpfCnpj", "CPF/CNPJ");
            grid.Columns.Add("Endereco", "Endereço");

            using var conn = Conexao.Abrir();
            string sql = "SELECT Nome, Telefone, CpfCnpj, Endereco FROM Clientes ORDER BY Nome";
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                grid.Rows.Add(reader["Nome"].ToString(), reader["Telefone"].ToString(), reader["CpfCnpj"].ToString(), reader["Endereco"].ToString());
            lblTotal.Text = $"Total de clientes: {grid.Rows.Count}";
        }

        private void RelatorioEstoque()
        {
            grid.Columns.Add("Codigo", "Código");
            grid.Columns.Add("Nome", "Nome");
            grid.Columns.Add("Estoque", "Estoque");
            grid.Columns.Add("Minimo", "Mínimo");
            grid.Columns.Add("Custo", "Custo R$");
            grid.Columns.Add("Venda", "Venda R$");

            using var conn = Conexao.Abrir();
            string sql = "SELECT * FROM Pecas ORDER BY Nome";
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                double qtd = Convert.ToDouble(reader["QuantidadeEstoque"]);
                double min = Convert.ToDouble(reader["EstoqueMinimo"]);
                int i = grid.Rows.Add(
                    reader["Codigo"].ToString(),
                    reader["Nome"].ToString(),
                    qtd.ToString("F2"),
                    min.ToString("F2"),
                    $"R$ {Convert.ToDouble(reader["ValorCusto"]):F2}",
                    $"R$ {Convert.ToDouble(reader["ValorVenda"]):F2}"
                );
                if (qtd <= min)
                    grid.Rows[i].DefaultCellStyle.BackColor = Color.LightCoral;
            }
            lblTotal.Text = $"Total de peças cadastradas: {grid.Rows.Count}";
        }

        private void RelatorioEstoqueBaixo()
        {
            grid.Columns.Add("Codigo", "Código");
            grid.Columns.Add("Nome", "Nome");
            grid.Columns.Add("Estoque", "Estoque Atual");
            grid.Columns.Add("Minimo", "Estoque Mínimo");

            using var conn = Conexao.Abrir();
            string sql = "SELECT Codigo, Nome, QuantidadeEstoque, EstoqueMinimo FROM Pecas WHERE QuantidadeEstoque <= EstoqueMinimo ORDER BY QuantidadeEstoque ASC";
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int i = grid.Rows.Add(
                    reader["Codigo"].ToString(),
                    reader["Nome"].ToString(),
                    Convert.ToDouble(reader["QuantidadeEstoque"]).ToString("F2"),
                    Convert.ToDouble(reader["EstoqueMinimo"]).ToString("F2")
                );
                grid.Rows[i].DefaultCellStyle.BackColor = Color.LightCoral;
            }
            lblTotal.Text = $"Peças com estoque baixo: {grid.Rows.Count}";
        }

        private void RelatorioFinanceiro(string inicio, string fim)
        {
            grid.Columns.Add("Tipo", "Tipo");
            grid.Columns.Add("Descricao", "Descrição");
            grid.Columns.Add("Valor", "Valor R$");
            grid.Columns.Add("Vencimento", "Vencimento");
            grid.Columns.Add("Status", "Status");

            double totalReceber = 0, totalPagar = 0;
            using var conn = Conexao.Abrir();
            string sql = @"SELECT Tipo, Descricao, Valor, DataVencimento, Status FROM Financeiro
                           WHERE DATE(DataVencimento) BETWEEN @ini AND @fim
                           ORDER BY DataVencimento ASC";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ini", inicio);
            cmd.Parameters.AddWithValue("@fim", fim);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string tipo = reader["Tipo"].ToString();
                double valor = Convert.ToDouble(reader["Valor"]);
                if (tipo == "Receber") totalReceber += valor;
                else totalPagar += valor;

                int i = grid.Rows.Add(
                    tipo,
                    reader["Descricao"].ToString(),
                    $"R$ {valor:F2}",
                    reader["DataVencimento"].ToString(),
                    reader["Status"].ToString()
                );
                grid.Rows[i].DefaultCellStyle.BackColor = tipo == "Pagar" ? Color.LightCoral : Color.LightYellow;
            }
            lblTotal.Text = $"A Receber: R$ {totalReceber:F2} | A Pagar: R$ {totalPagar:F2} | Saldo: R$ {totalReceber - totalPagar:F2}";
        }
    }
}