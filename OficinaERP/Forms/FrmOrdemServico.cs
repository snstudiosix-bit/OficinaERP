using Microsoft.Data.Sqlite;
using OficinaERP.Database;
using OficinaERP.Relatorios;
using System;
using System.Windows.Forms;

namespace OficinaERP.Forms
{
    public class FrmOrdemServico : Form
    {
        private ComboBox cmbCliente = new ComboBox();
        private ComboBox cmbVeiculo = new ComboBox();
        private ComboBox cmbStatus = new ComboBox();
        private TextBox txtDefeitoInformado = new TextBox();
        private TextBox txtDiagnostico = new TextBox();
        private TextBox txtServicos = new TextBox();
        private TextBox txtPecas = new TextBox();
        private TextBox txtMaoDeObra = new TextBox();
        private TextBox txtValorTotal = new TextBox();
        private TextBox txtObservacoes = new TextBox();
        private TextBox txtPesquisa = new TextBox();
        private Button btnSalvar = new Button();
        private Button btnNovo = new Button();
        private Button btnExcluir = new Button();
        private Button btnPesquisar = new Button();
        private Button btnPdf = new Button();
        private DataGridView grid = new DataGridView();
        private int idSelecionado = 0;

        public FrmOrdemServico()
        {
            Text = "Ordens de Serviço";
            Width = 860;
            Height = 720;
            StartPosition = FormStartPosition.CenterScreen;
            AutoScroll = true;

            int col1 = 20, col2 = 130, largura = 280;

            var lblCliente = new Label() { Text = "Cliente:", Left = col1, Top = 20, Width = 100 };
            cmbCliente.Left = col2; cmbCliente.Top = 18; cmbCliente.Width = largura;
            cmbCliente.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCliente.SelectedIndexChanged += CmbCliente_SelectedIndexChanged;

            var lblVeiculo = new Label() { Text = "Veículo:", Left = col1, Top = 55, Width = 100 };
            cmbVeiculo.Left = col2; cmbVeiculo.Top = 53; cmbVeiculo.Width = largura;
            cmbVeiculo.DropDownStyle = ComboBoxStyle.DropDownList;

            var lblStatus = new Label() { Text = "Status:", Left = col1, Top = 90, Width = 100 };
            cmbStatus.Left = col2; cmbStatus.Top = 88; cmbStatus.Width = 180;
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Items.AddRange(new string[] { "Aberta", "Em andamento", "Concluída", "Entregue" });
            cmbStatus.SelectedIndex = 0;

            var lblDefeito = new Label() { Text = "Defeito:", Left = col1, Top = 125, Width = 100 };
            txtDefeitoInformado.Left = col2; txtDefeitoInformado.Top = 123; txtDefeitoInformado.Width = largura;
            txtDefeitoInformado.Multiline = true; txtDefeitoInformado.Height = 50;

            var lblDiagnostico = new Label() { Text = "Diagnóstico:", Left = col1, Top = 185, Width = 100 };
            txtDiagnostico.Left = col2; txtDiagnostico.Top = 183; txtDiagnostico.Width = largura;
            txtDiagnostico.Multiline = true; txtDiagnostico.Height = 50;

            var lblServicos = new Label() { Text = "Serviços:", Left = col1, Top = 245, Width = 100 };
            txtServicos.Left = col2; txtServicos.Top = 243; txtServicos.Width = largura;
            txtServicos.Multiline = true; txtServicos.Height = 50;

            var lblPecas = new Label() { Text = "Peças:", Left = col1, Top = 305, Width = 100 };
            txtPecas.Left = col2; txtPecas.Top = 303; txtPecas.Width = largura;
            txtPecas.Multiline = true; txtPecas.Height = 50;

            var lblMaoDeObra = new Label() { Text = "Mão de obra R$:", Left = col1, Top = 365, Width = 110 };
            txtMaoDeObra.Left = col2; txtMaoDeObra.Top = 363; txtMaoDeObra.Width = 120;
            txtMaoDeObra.Leave += TxtValor_Leave;

            var lblValorTotal = new Label() { Text = "Valor total R$:", Left = col1, Top = 400, Width = 110 };
            txtValorTotal.Left = col2; txtValorTotal.Top = 398; txtValorTotal.Width = 120;
            txtValorTotal.Leave += TxtValor_Leave;

            var lblObs = new Label() { Text = "Observações:", Left = col1, Top = 435, Width = 110 };
            txtObservacoes.Left = col2; txtObservacoes.Top = 433; txtObservacoes.Width = largura;
            txtObservacoes.Multiline = true; txtObservacoes.Height = 40;

            btnSalvar.Text = "Salvar";
            btnSalvar.Left = col2; btnSalvar.Top = 485; btnSalvar.Width = 100;
            btnSalvar.Click += BtnSalvar_Click;

            btnNovo.Text = "Novo";
            btnNovo.Left = col2 + 110; btnNovo.Top = 485; btnNovo.Width = 100;
            btnNovo.Click += BtnNovo_Click;

            btnExcluir.Text = "Excluir";
            btnExcluir.Left = col2 + 220; btnExcluir.Top = 485; btnExcluir.Width = 100;
            btnExcluir.Click += BtnExcluir_Click;

            // ✅ CORRIGIDO - removido o "F" sobrando
            btnPdf.Text = "PDF";
            btnPdf.Left = col2 + 330; btnPdf.Top = 485; btnPdf.Width = 100;
            btnPdf.Click += BtnPdf_Click;

            var lblPesquisa = new Label() { Text = "Pesquisar:", Left = col1, Top = 530, Width = 100 };
            txtPesquisa.Left = col2; txtPesquisa.Top = 528; txtPesquisa.Width = 200;
            txtPesquisa.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) CarregarOS(txtPesquisa.Text); };

            btnPesquisar.Text = "Buscar";
            btnPesquisar.Left = col2 + 210; btnPesquisar.Top = 526; btnPesquisar.Width = 80;
            btnPesquisar.Click += BtnPesquisar_Click;

            grid.Left = col1; grid.Top = 565;
            grid.Width = 800; grid.Height = 100;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.CellClick += Grid_CellClick;

            // ✅ CORRIGIDO - btnPdf adicionado ao Controls.AddRange
            Controls.AddRange(new Control[] {
                lblCliente, cmbCliente,
                lblVeiculo, cmbVeiculo,
                lblStatus, cmbStatus,
                lblDefeito, txtDefeitoInformado,
                lblDiagnostico, txtDiagnostico,
                lblServicos, txtServicos,
                lblPecas, txtPecas,
                lblMaoDeObra, txtMaoDeObra,
                lblValorTotal, txtValorTotal,
                lblObs, txtObservacoes,
                btnSalvar, btnNovo, btnExcluir, btnPdf,
                lblPesquisa, txtPesquisa, btnPesquisar,
                grid
            });

            CarregarClientes();
            CarregarOS("");
        }

        private void CarregarClientes()
        {
            cmbCliente.Items.Clear();
            using var conn = Conexao.Abrir();
            string sql = "SELECT Id, Nome FROM Clientes ORDER BY Nome";
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                cmbCliente.Items.Add(new { Id = reader["Id"].ToString(), Nome = reader["Nome"].ToString() });
            cmbCliente.DisplayMember = "Nome";
        }

        private void CmbCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbVeiculo.Items.Clear();
            if (cmbCliente.SelectedItem == null) return;
            dynamic clienteSelecionado = cmbCliente.SelectedItem;
            using var conn = Conexao.Abrir();
            string sql = "SELECT Id, Placa, Modelo FROM Veiculos WHERE ClienteId = @cid";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@cid", clienteSelecionado.Id);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                cmbVeiculo.Items.Add(new { Id = reader["Id"].ToString(), Nome = $"{reader["Placa"]} - {reader["Modelo"]}" });
            cmbVeiculo.DisplayMember = "Nome";
        }

        private void CarregarOS(string filtro)
        {
            grid.Rows.Clear();
            grid.Columns.Clear();
            grid.Columns.Add("Id", "OS");
            grid.Columns.Add("Data", "Data");
            grid.Columns.Add("Cliente", "Cliente");
            grid.Columns.Add("Veiculo", "Veículo");
            grid.Columns.Add("Status", "Status");
            grid.Columns.Add("ValorTotal", "Total R$");
            grid.Columns.Add("ClienteId", "ClienteId");
            grid.Columns.Add("VeiculoId", "VeiculoId");
            grid.Columns["ClienteId"].Visible = false;
            grid.Columns["VeiculoId"].Visible = false;

            using var conn = Conexao.Abrir();
            string sql = @"
                SELECT os.Id, os.DataAbertura, c.Nome, v.Placa, os.Status, os.ValorTotal,
                       os.ClienteId, os.VeiculoId, os.DefeitoInformado, os.Diagnostico,
                       os.ServicosExecutados, os.PecasUtilizadas, os.ValorMaoDeObra, os.Observacoes
                FROM OrdensServico os
                INNER JOIN Clientes c ON c.Id = os.ClienteId
                INNER JOIN Veiculos v ON v.Id = os.VeiculoId
                WHERE c.Nome LIKE @filtro OR v.Placa LIKE @filtro
                ORDER BY os.Id DESC
            ";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@filtro", $"%{filtro}%");
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                grid.Rows.Add(
                    reader["Id"].ToString(),
                    reader["DataAbertura"].ToString(),
                    reader["Nome"].ToString(),
                    reader["Placa"].ToString(),
                    reader["Status"].ToString(),
                    $"R$ {reader["ValorTotal"]:F2}",
                    reader["ClienteId"].ToString(),
                    reader["VeiculoId"].ToString()
                );
            }
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = grid.Rows[e.RowIndex];
            idSelecionado = int.Parse(row.Cells["Id"].Value.ToString());

            using var conn = Conexao.Abrir();
            string sql = "SELECT * FROM OrdensServico WHERE Id = @id";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", idSelecionado);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                txtDefeitoInformado.Text = reader["DefeitoInformado"].ToString();
                txtDiagnostico.Text = reader["Diagnostico"].ToString();
                txtServicos.Text = reader["ServicosExecutados"].ToString();
                txtPecas.Text = reader["PecasUtilizadas"].ToString();
                txtMaoDeObra.Text = reader["ValorMaoDeObra"].ToString();
                txtValorTotal.Text = reader["ValorTotal"].ToString();
                txtObservacoes.Text = reader["Observacoes"].ToString();
                cmbStatus.SelectedItem = reader["Status"].ToString();

                int clienteId = int.Parse(reader["ClienteId"].ToString());
                int veiculoId = int.Parse(reader["VeiculoId"].ToString());

                foreach (dynamic item in cmbCliente.Items)
                {
                    if (item.Id == clienteId.ToString())
                    {
                        cmbCliente.SelectedItem = item;
                        break;
                    }
                }

                foreach (dynamic item in cmbVeiculo.Items)
                {
                    if (item.Id == veiculoId.ToString())
                    {
                        cmbVeiculo.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void TxtValor_Leave(object sender, EventArgs e)
        {
            var txt = sender as TextBox;
            if (double.TryParse(txt.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double val))
                txt.Text = val.ToString("F2");
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (cmbCliente.SelectedItem == null || cmbVeiculo.SelectedItem == null)
            {
                MessageBox.Show("Selecione o cliente e o veículo!", "Atenção");
                return;
            }

            dynamic cliente = cmbCliente.SelectedItem;
            dynamic veiculo = cmbVeiculo.SelectedItem;

            double.TryParse(txtMaoDeObra.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double mdo);
            double.TryParse(txtValorTotal.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double total);

            using var conn = Conexao.Abrir();

            if (idSelecionado == 0)
            {
                string sql = @"INSERT INTO OrdensServico
                    (ClienteId, VeiculoId, DataAbertura, DefeitoInformado, Diagnostico,
                     ServicosExecutados, PecasUtilizadas, ValorMaoDeObra, ValorTotal, Status, Observacoes)
                    VALUES (@cid, @vid, @data, @defeito, @diag, @serv, @pecas, @mdo, @total, @status, @obs)";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cid", cliente.Id);
                cmd.Parameters.AddWithValue("@vid", veiculo.Id);
                cmd.Parameters.AddWithValue("@data", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                cmd.Parameters.AddWithValue("@defeito", txtDefeitoInformado.Text);
                cmd.Parameters.AddWithValue("@diag", txtDiagnostico.Text);
                cmd.Parameters.AddWithValue("@serv", txtServicos.Text);
                cmd.Parameters.AddWithValue("@pecas", txtPecas.Text);
                cmd.Parameters.AddWithValue("@mdo", mdo);
                cmd.Parameters.AddWithValue("@total", total);
                cmd.Parameters.AddWithValue("@status", cmbStatus.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@obs", txtObservacoes.Text);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Ordem de Serviço aberta com sucesso!", "Sucesso");
            }
            else
            {
                string sql = @"UPDATE OrdensServico SET
                    ClienteId=@cid, VeiculoId=@vid, DefeitoInformado=@defeito, Diagnostico=@diag,
                    ServicosExecutados=@serv, PecasUtilizadas=@pecas, ValorMaoDeObra=@mdo,
                    ValorTotal=@total, Status=@status, Observacoes=@obs WHERE Id=@id";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cid", cliente.Id);
                cmd.Parameters.AddWithValue("@vid", veiculo.Id);
                cmd.Parameters.AddWithValue("@defeito", txtDefeitoInformado.Text);
                cmd.Parameters.AddWithValue("@diag", txtDiagnostico.Text);
                cmd.Parameters.AddWithValue("@serv", txtServicos.Text);
                cmd.Parameters.AddWithValue("@pecas", txtPecas.Text);
                cmd.Parameters.AddWithValue("@mdo", mdo);
                cmd.Parameters.AddWithValue("@total", total);
                cmd.Parameters.AddWithValue("@status", cmbStatus.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@obs", txtObservacoes.Text);
                cmd.Parameters.AddWithValue("@id", idSelecionado);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Ordem de Serviço atualizada!", "Sucesso");
            }

            LimparCampos();
            CarregarOS("");
        }

        private void BtnNovo_Click(object sender, EventArgs e) => LimparCampos();

        private void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (idSelecionado == 0)
            {
                MessageBox.Show("Selecione uma OS na tabela primeiro!", "Atenção");
                return;
            }
            var confirm = MessageBox.Show("Deseja excluir esta OS?", "Confirmar", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                using var conn = Conexao.Abrir();
                string sql = "DELETE FROM OrdensServico WHERE Id=@id";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", idSelecionado);
                cmd.ExecuteNonQuery();
                MessageBox.Show("OS excluída!", "Sucesso");
                LimparCampos();
                CarregarOS("");
            }
        }

        private void BtnPesquisar_Click(object sender, EventArgs e) => CarregarOS(txtPesquisa.Text);

        // ✅ ADICIONADO - método do botão PDF
        private void BtnPdf_Click(object sender, EventArgs e)
        {
            if (idSelecionado == 0)
            {
                MessageBox.Show("Selecione uma OS para gerar o PDF.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = grid.CurrentRow;
            if (row == null) return;

            string cliente = row.Cells["Cliente"].Value.ToString();
            string veiculo = row.Cells["Veiculo"].Value.ToString();
            string data = row.Cells["Data"].Value.ToString();

            PdfHelper.GerarOrdemServico(
                numeroOS: idSelecionado,
                dataAbertura: data,
                cliente: cliente,
                placa: veiculo,
                marca: "", modelo: "", ano: "", cor: "",
                defeito: txtDefeitoInformado.Text,
                diagnostico: txtDiagnostico.Text,
                servicosExecutados: txtServicos.Text,
                pecasUtilizadas: txtPecas.Text,
                maodeObra: txtMaoDeObra.Text,
                valorTotal: txtValorTotal.Text,
                observacoes: txtObservacoes.Text
            );

            MessageBox.Show($"PDF gerado com sucesso!\nOS_{idSelecionado:D3}.pdf",
                "PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LimparCampos()
        {
            idSelecionado = 0;
            cmbCliente.SelectedIndex = -1;
            cmbVeiculo.Items.Clear();
            cmbStatus.SelectedIndex = 0;
            txtDefeitoInformado.Clear();
            txtDiagnostico.Clear();
            txtServicos.Clear();
            txtPecas.Clear();
            txtMaoDeObra.Clear();
            txtValorTotal.Clear();
            txtObservacoes.Clear();
        }
    }
}