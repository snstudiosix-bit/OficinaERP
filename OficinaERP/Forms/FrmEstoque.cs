using Microsoft.Data.Sqlite;
using OficinaERP.Database;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OficinaERP.Forms
{
    public class FrmEstoque : Form
    {
        private TextBox txtNome = new TextBox();
        private TextBox txtCodigo = new TextBox();
        private TextBox txtDescricao = new TextBox();
        private TextBox txtEstoqueMinimo = new TextBox();
        private TextBox txtValorCusto = new TextBox();
        private TextBox txtValorVenda = new TextBox();
        private TextBox txtPesquisa = new TextBox();
        private TextBox txtQtdMovimento = new TextBox();
        private TextBox txtObsMovimento = new TextBox();
        private ComboBox cmbTipoMovimento = new ComboBox();
        private Button btnSalvar = new Button();
        private Button btnNovo = new Button();
        private Button btnExcluir = new Button();
        private Button btnPesquisar = new Button();
        private Button btnMovimentar = new Button();
        private DataGridView grid = new DataGridView();
        private Label lblEstoqueAtual = new Label();
        private int idSelecionado = 0;

        public FrmEstoque()
        {
            Text = "Controle de Estoque";
            Width = 860;
            Height = 680;
            StartPosition = FormStartPosition.CenterScreen;
            AutoScroll = true;

            int col1 = 20, col2 = 130, larg = 250;

            var lblNome = new Label() { Text = "Nome:", Left = col1, Top = 20, Width = 100 };
            txtNome.Left = col2; txtNome.Top = 18; txtNome.Width = larg;

            var lblCodigo = new Label() { Text = "Código:", Left = col1, Top = 55, Width = 100 };
            txtCodigo.Left = col2; txtCodigo.Top = 53; txtCodigo.Width = 150;

            var lblDescricao = new Label() { Text = "Descrição:", Left = col1, Top = 90, Width = 100 };
            txtDescricao.Left = col2; txtDescricao.Top = 88; txtDescricao.Width = larg;

            var lblEstMin = new Label() { Text = "Estoque mín.:", Left = col1, Top = 125, Width = 100 };
            txtEstoqueMinimo.Left = col2; txtEstoqueMinimo.Top = 123; txtEstoqueMinimo.Width = 100;

            var lblCusto = new Label() { Text = "Custo R$:", Left = col1, Top = 160, Width = 100 };
            txtValorCusto.Left = col2; txtValorCusto.Top = 158; txtValorCusto.Width = 120;

            var lblVenda = new Label() { Text = "Venda R$:", Left = col1, Top = 195, Width = 100 };
            txtValorVenda.Left = col2; txtValorVenda.Top = 193; txtValorVenda.Width = 120;

            btnSalvar.Text = "Salvar";
            btnSalvar.Left = col2; btnSalvar.Top = 235; btnSalvar.Width = 100;
            btnSalvar.Click += BtnSalvar_Click;

            btnNovo.Text = "Novo";
            btnNovo.Left = col2 + 110; btnNovo.Top = 235; btnNovo.Width = 100;
            btnNovo.Click += BtnNovo_Click;

            btnExcluir.Text = "Excluir";
            btnExcluir.Left = col2 + 220; btnExcluir.Top = 235; btnExcluir.Width = 100;
            btnExcluir.Click += BtnExcluir_Click;

            // Painel de movimentação
            var lblSep = new Label()
            {
                Text = "─── Movimentação de Estoque ───",
                Left = col1,
                Top = 285,
                Width = 400,
                ForeColor = Color.DarkBlue,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            lblEstoqueAtual.Left = col1; lblEstoqueAtual.Top = 310;
            lblEstoqueAtual.Width = 300;
            lblEstoqueAtual.Font = new Font("Arial", 9, FontStyle.Bold);
            lblEstoqueAtual.ForeColor = Color.DarkGreen;

            var lblTipo = new Label() { Text = "Tipo:", Left = col1, Top = 335, Width = 100 };
            cmbTipoMovimento.Left = col2; cmbTipoMovimento.Top = 333; cmbTipoMovimento.Width = 150;
            cmbTipoMovimento.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTipoMovimento.Items.AddRange(new string[] { "Entrada", "Saída" });
            cmbTipoMovimento.SelectedIndex = 0;

            var lblQtd = new Label() { Text = "Quantidade:", Left = col1, Top = 370, Width = 100 };
            txtQtdMovimento.Left = col2; txtQtdMovimento.Top = 368; txtQtdMovimento.Width = 100;

            var lblObsMov = new Label() { Text = "Observação:", Left = col1, Top = 405, Width = 100 };
            txtObsMovimento.Left = col2; txtObsMovimento.Top = 403; txtObsMovimento.Width = larg;

            btnMovimentar.Text = "Registrar Movimentação";
            btnMovimentar.Left = col2; btnMovimentar.Top = 440; btnMovimentar.Width = 200;
            btnMovimentar.BackColor = Color.DarkBlue;
            btnMovimentar.ForeColor = Color.White;
            btnMovimentar.Click += BtnMovimentar_Click;

            var lblPesquisa = new Label() { Text = "Pesquisar:", Left = col1, Top = 490, Width = 100 };
            txtPesquisa.Left = col2; txtPesquisa.Top = 488; txtPesquisa.Width = 200;

            btnPesquisar.Text = "Buscar";
            btnPesquisar.Left = col2 + 210; btnPesquisar.Top = 486; btnPesquisar.Width = 80;
            btnPesquisar.Click += BtnPesquisar_Click;

            grid.Left = col1; grid.Top = 525;
            grid.Width = 800; grid.Height = 110;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.CellClick += Grid_CellClick;

            Controls.AddRange(new Control[] {
                lblNome, txtNome,
                lblCodigo, txtCodigo,
                lblDescricao, txtDescricao,
                lblEstMin, txtEstoqueMinimo,
                lblCusto, txtValorCusto,
                lblVenda, txtValorVenda,
                btnSalvar, btnNovo, btnExcluir,
                lblSep, lblEstoqueAtual,
                lblTipo, cmbTipoMovimento,
                lblQtd, txtQtdMovimento,
                lblObsMov, txtObsMovimento,
                btnMovimentar,
                lblPesquisa, txtPesquisa, btnPesquisar,
                grid
            });

            CarregarPecas("");
        }

        private void CarregarPecas(string filtro)
        {
            grid.Rows.Clear();
            grid.Columns.Clear();
            grid.Columns.Add("Id", "ID");
            grid.Columns.Add("Codigo", "Código");
            grid.Columns.Add("Nome", "Nome");
            grid.Columns.Add("Estoque", "Estoque");
            grid.Columns.Add("EstMin", "Est. Mín.");
            grid.Columns.Add("Custo", "Custo R$");
            grid.Columns.Add("Venda", "Venda R$");
            grid.Columns.Add("Alerta", "");
            grid.Columns["Id"].Visible = false;

            using var conn = Conexao.Abrir();
            string sql = "SELECT * FROM Pecas WHERE Nome LIKE @filtro OR Codigo LIKE @filtro ORDER BY Nome";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@filtro", $"%{filtro}%");
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                double qtd = Convert.ToDouble(reader["QuantidadeEstoque"]);
                double min = Convert.ToDouble(reader["EstoqueMinimo"]);
                string alerta = qtd <= min ? "⚠️ Baixo" : "✅ OK";

                int rowIndex = grid.Rows.Add(
                    reader["Id"].ToString(),
                    reader["Codigo"].ToString(),
                    reader["Nome"].ToString(),
                    qtd.ToString("F2"),
                    min.ToString("F2"),
                    $"R$ {Convert.ToDouble(reader["ValorCusto"]):F2}",
                    $"R$ {Convert.ToDouble(reader["ValorVenda"]):F2}",
                    alerta
                );

                if (qtd <= min)
                    grid.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
            }
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = grid.Rows[e.RowIndex];
            idSelecionado = int.Parse(row.Cells["Id"].Value.ToString());
            txtCodigo.Text = row.Cells["Codigo"].Value.ToString();
            txtNome.Text = row.Cells["Nome"].Value.ToString();
            txtEstoqueMinimo.Text = row.Cells["EstMin"].Value.ToString();
            txtValorCusto.Text = row.Cells["Custo"].Value.ToString().Replace("R$ ", "");
            txtValorVenda.Text = row.Cells["Venda"].Value.ToString().Replace("R$ ", "");

            using var conn = Conexao.Abrir();
            string sql = "SELECT Descricao, QuantidadeEstoque FROM Pecas WHERE Id = @id";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", idSelecionado);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                txtDescricao.Text = reader["Descricao"].ToString();
                double qtd = Convert.ToDouble(reader["QuantidadeEstoque"]);
                lblEstoqueAtual.Text = $"Estoque atual: {qtd:F2} unidades";
                lblEstoqueAtual.ForeColor = qtd <= 0 ? Color.Red : Color.DarkGreen;
            }
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("O nome da peça é obrigatório!", "Atenção");
                return;
            }

            double.TryParse(txtEstoqueMinimo.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double estMin);
            double.TryParse(txtValorCusto.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double custo);
            double.TryParse(txtValorVenda.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double venda);

            using var conn = Conexao.Abrir();

            if (idSelecionado == 0)
            {
                string sql = @"INSERT INTO Pecas (Nome, Codigo, Descricao, EstoqueMinimo, ValorCusto, ValorVenda)
                               VALUES (@nome, @cod, @desc, @estmin, @custo, @venda)";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", txtNome.Text);
                cmd.Parameters.AddWithValue("@cod", txtCodigo.Text);
                cmd.Parameters.AddWithValue("@desc", txtDescricao.Text);
                cmd.Parameters.AddWithValue("@estmin", estMin);
                cmd.Parameters.AddWithValue("@custo", custo);
                cmd.Parameters.AddWithValue("@venda", venda);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Peça cadastrada com sucesso!", "Sucesso");
            }
            else
            {
                string sql = @"UPDATE Pecas SET Nome=@nome, Codigo=@cod, Descricao=@desc,
                               EstoqueMinimo=@estmin, ValorCusto=@custo, ValorVenda=@venda WHERE Id=@id";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", txtNome.Text);
                cmd.Parameters.AddWithValue("@cod", txtCodigo.Text);
                cmd.Parameters.AddWithValue("@desc", txtDescricao.Text);
                cmd.Parameters.AddWithValue("@estmin", estMin);
                cmd.Parameters.AddWithValue("@custo", custo);
                cmd.Parameters.AddWithValue("@venda", venda);
                cmd.Parameters.AddWithValue("@id", idSelecionado);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Peça atualizada!", "Sucesso");
            }

            LimparCampos();
            CarregarPecas("");
        }

        private void BtnMovimentar_Click(object sender, EventArgs e)
        {
            if (idSelecionado == 0)
            {
                MessageBox.Show("Selecione uma peça na tabela primeiro!", "Atenção");
                return;
            }

            if (!double.TryParse(txtQtdMovimento.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double qtd) || qtd <= 0)
            {
                MessageBox.Show("Informe uma quantidade válida!", "Atenção");
                return;
            }

            string tipo = cmbTipoMovimento.SelectedItem.ToString();

            using var conn = Conexao.Abrir();

            if (tipo == "Saída")
            {
                string sqlQtd = "SELECT QuantidadeEstoque FROM Pecas WHERE Id = @id";
                using var cmdQtd = new SqliteCommand(sqlQtd, conn);
                cmdQtd.Parameters.AddWithValue("@id", idSelecionado);
                double estoqueAtual = Convert.ToDouble(cmdQtd.ExecuteScalar());
                if (qtd > estoqueAtual)
                {
                    MessageBox.Show($"Estoque insuficiente! Disponível: {estoqueAtual:F2}", "Atenção");
                    return;
                }
            }

            string sqlMov = @"INSERT INTO MovimentacaoEstoque (PecaId, Tipo, Quantidade, Data, Observacao)
                              VALUES (@pid, @tipo, @qtd, @data, @obs)";
            using var cmdMov = new SqliteCommand(sqlMov, conn);
            cmdMov.Parameters.AddWithValue("@pid", idSelecionado);
            cmdMov.Parameters.AddWithValue("@tipo", tipo);
            cmdMov.Parameters.AddWithValue("@qtd", qtd);
            cmdMov.Parameters.AddWithValue("@data", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            cmdMov.Parameters.AddWithValue("@obs", txtObsMovimento.Text);
            cmdMov.ExecuteNonQuery();

            string sinal = tipo == "Entrada" ? "+" : "-";
            string sqlAtualiza = $"UPDATE Pecas SET QuantidadeEstoque = QuantidadeEstoque {sinal} @qtd WHERE Id = @id";
            using var cmdAtualiza = new SqliteCommand(sqlAtualiza, conn);
            cmdAtualiza.Parameters.AddWithValue("@qtd", qtd);
            cmdAtualiza.Parameters.AddWithValue("@id", idSelecionado);
            cmdAtualiza.ExecuteNonQuery();

            MessageBox.Show($"{tipo} registrada com sucesso!", "Sucesso");
            txtQtdMovimento.Clear();
            txtObsMovimento.Clear();
            LimparCampos();
            CarregarPecas("");
        }

        private void BtnNovo_Click(object sender, EventArgs e) => LimparCampos();

        private void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (idSelecionado == 0)
            {
                MessageBox.Show("Selecione uma peça na tabela primeiro!", "Atenção");
                return;
            }
            var confirm = MessageBox.Show("Deseja excluir esta peça?", "Confirmar", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                using var conn = Conexao.Abrir();
                string sql = "DELETE FROM Pecas WHERE Id=@id";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", idSelecionado);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Peça excluída!", "Sucesso");
                LimparCampos();
                CarregarPecas("");
            }
        }

        private void BtnPesquisar_Click(object sender, EventArgs e) => CarregarPecas(txtPesquisa.Text);

        private void LimparCampos()
        {
            idSelecionado = 0;
            txtNome.Clear();
            txtCodigo.Clear();
            txtDescricao.Clear();
            txtEstoqueMinimo.Clear();
            txtValorCusto.Clear();
            txtValorVenda.Clear();
            lblEstoqueAtual.Text = "";
        }
    }
}