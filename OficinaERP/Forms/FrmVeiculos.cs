using Microsoft.Data.Sqlite;
using OficinaERP.Database;
using System;
using System.Windows.Forms;

namespace OficinaERP.Forms
{
    public class FrmVeiculos : Form
    {
        private ComboBox cmbCliente = new ComboBox();
        private TextBox txtPlaca = new TextBox();
        private TextBox txtMarca = new TextBox();
        private TextBox txtModelo = new TextBox();
        private TextBox txtAno = new TextBox();
        private TextBox txtCor = new TextBox();
        private TextBox txtQuilometragem = new TextBox();
        private TextBox txtPesquisa = new TextBox();
        private Button btnSalvar = new Button();
        private Button btnNovo = new Button();
        private Button btnExcluir = new Button();
        private Button btnPesquisar = new Button();
        private DataGridView grid = new DataGridView();
        private int idSelecionado = 0;

        public FrmVeiculos()
        {
            Text = "Cadastro de Veículos";
            Width = 800;
            Height = 620;
            StartPosition = FormStartPosition.CenterScreen;

            var lblCliente = new Label() { Text = "Cliente:", Left = 20, Top = 20, Width = 80 };
            cmbCliente.Left = 110; cmbCliente.Top = 18; cmbCliente.Width = 250;
            cmbCliente.DropDownStyle = ComboBoxStyle.DropDownList;

            var lblPlaca = new Label() { Text = "Placa:", Left = 20, Top = 55, Width = 80 };
            txtPlaca.Left = 110; txtPlaca.Top = 53; txtPlaca.Width = 150;

            var lblMarca = new Label() { Text = "Marca:", Left = 20, Top = 90, Width = 80 };
            txtMarca.Left = 110; txtMarca.Top = 88; txtMarca.Width = 200;

            var lblModelo = new Label() { Text = "Modelo:", Left = 20, Top = 125, Width = 80 };
            txtModelo.Left = 110; txtModelo.Top = 123; txtModelo.Width = 200;

            var lblAno = new Label() { Text = "Ano:", Left = 20, Top = 160, Width = 80 };
            txtAno.Left = 110; txtAno.Top = 158; txtAno.Width = 100;

            var lblCor = new Label() { Text = "Cor:", Left = 20, Top = 195, Width = 80 };
            txtCor.Left = 110; txtCor.Top = 193; txtCor.Width = 150;

            var lblKm = new Label() { Text = "KM:", Left = 20, Top = 230, Width = 80 };
            txtQuilometragem.Left = 110; txtQuilometragem.Top = 228; txtQuilometragem.Width = 150;

            btnSalvar.Text = "Salvar";
            btnSalvar.Left = 110; btnSalvar.Top = 270; btnSalvar.Width = 100;
            btnSalvar.Click += BtnSalvar_Click;

            btnNovo.Text = "Novo";
            btnNovo.Left = 220; btnNovo.Top = 270; btnNovo.Width = 100;
            btnNovo.Click += BtnNovo_Click;

            btnExcluir.Text = "Excluir";
            btnExcluir.Left = 330; btnExcluir.Top = 270; btnExcluir.Width = 100;
            btnExcluir.Click += BtnExcluir_Click;

            var lblPesquisa = new Label() { Text = "Pesquisar:", Left = 20, Top = 315, Width = 80 };
            txtPesquisa.Left = 110; txtPesquisa.Top = 313; txtPesquisa.Width = 200;

            btnPesquisar.Text = "Buscar";
            btnPesquisar.Left = 320; btnPesquisar.Top = 311; btnPesquisar.Width = 80;
            btnPesquisar.Click += BtnPesquisar_Click;

            grid.Left = 20; grid.Top = 350;
            grid.Width = 740; grid.Height = 220;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.CellClick += Grid_CellClick;

            Controls.AddRange(new Control[] {
                lblCliente, cmbCliente,
                lblPlaca, txtPlaca,
                lblMarca, txtMarca,
                lblModelo, txtModelo,
                lblAno, txtAno,
                lblCor, txtCor,
                lblKm, txtQuilometragem,
                btnSalvar, btnNovo, btnExcluir,
                lblPesquisa, txtPesquisa, btnPesquisar,
                grid
            });

            CarregarClientes();
            CarregarVeiculos("");
        }

        private void CarregarClientes()
        {
            cmbCliente.Items.Clear();
            using var conn = Conexao.Abrir();
            string sql = "SELECT Id, Nome FROM Clientes ORDER BY Nome";
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cmbCliente.Items.Add(new { Id = reader["Id"].ToString(), Nome = reader["Nome"].ToString() });
            }
            cmbCliente.DisplayMember = "Nome";
        }

        private void CarregarVeiculos(string filtro)
        {
            grid.Rows.Clear();
            grid.Columns.Clear();
            grid.Columns.Add("Id", "ID");
            grid.Columns.Add("Cliente", "Cliente");
            grid.Columns.Add("Placa", "Placa");
            grid.Columns.Add("Marca", "Marca");
            grid.Columns.Add("Modelo", "Modelo");
            grid.Columns.Add("Ano", "Ano");
            grid.Columns.Add("Cor", "Cor");
            grid.Columns.Add("Km", "KM");
            grid.Columns.Add("ClienteId", "ClienteId");
            grid.Columns["Id"].Visible = false;
            grid.Columns["ClienteId"].Visible = false;

            using var conn = Conexao.Abrir();
            string sql = @"
                SELECT v.Id, c.Nome, v.Placa, v.Marca, v.Modelo, v.Ano, v.Cor, v.Quilometragem, v.ClienteId
                FROM Veiculos v
                INNER JOIN Clientes c ON c.Id = v.ClienteId
                WHERE v.Placa LIKE @filtro OR c.Nome LIKE @filtro
            ";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@filtro", $"%{filtro}%");
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                grid.Rows.Add(
                    reader["Id"].ToString(),
                    reader["Nome"].ToString(),
                    reader["Placa"].ToString(),
                    reader["Marca"].ToString(),
                    reader["Modelo"].ToString(),
                    reader["Ano"].ToString(),
                    reader["Cor"].ToString(),
                    reader["Quilometragem"].ToString(),
                    reader["ClienteId"].ToString()
                );
            }
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = grid.Rows[e.RowIndex];
            idSelecionado = int.Parse(row.Cells["Id"].Value.ToString());
            txtPlaca.Text = row.Cells["Placa"].Value.ToString();
            txtMarca.Text = row.Cells["Marca"].Value.ToString();
            txtModelo.Text = row.Cells["Modelo"].Value.ToString();
            txtAno.Text = row.Cells["Ano"].Value.ToString();
            txtCor.Text = row.Cells["Cor"].Value.ToString();
            txtQuilometragem.Text = row.Cells["Km"].Value.ToString();

            int clienteId = int.Parse(row.Cells["ClienteId"].Value.ToString());
            foreach (dynamic item in cmbCliente.Items)
            {
                if (item.Id == clienteId.ToString())
                {
                    cmbCliente.SelectedItem = item;
                    break;
                }
            }
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (cmbCliente.SelectedItem == null)
            {
                MessageBox.Show("Selecione um cliente!", "Atenção");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPlaca.Text))
            {
                MessageBox.Show("A placa é obrigatória!", "Atenção");
                return;
            }

            dynamic clienteSelecionado = cmbCliente.SelectedItem;
            int clienteId = int.Parse(clienteSelecionado.Id);

            using var conn = Conexao.Abrir();

            if (idSelecionado == 0)
            {
                string sql = @"INSERT INTO Veiculos (ClienteId, Placa, Marca, Modelo, Ano, Cor, Quilometragem)
                               VALUES (@cid, @placa, @marca, @modelo, @ano, @cor, @km)";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cid", clienteId);
                cmd.Parameters.AddWithValue("@placa", txtPlaca.Text.ToUpper());
                cmd.Parameters.AddWithValue("@marca", txtMarca.Text);
                cmd.Parameters.AddWithValue("@modelo", txtModelo.Text);
                cmd.Parameters.AddWithValue("@ano", txtAno.Text);
                cmd.Parameters.AddWithValue("@cor", txtCor.Text);
                cmd.Parameters.AddWithValue("@km", txtQuilometragem.Text);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Veículo cadastrado com sucesso!", "Sucesso");
            }
            else
            {
                string sql = @"UPDATE Veiculos SET ClienteId=@cid, Placa=@placa, Marca=@marca,
                               Modelo=@modelo, Ano=@ano, Cor=@cor, Quilometragem=@km WHERE Id=@id";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cid", clienteId);
                cmd.Parameters.AddWithValue("@placa", txtPlaca.Text.ToUpper());
                cmd.Parameters.AddWithValue("@marca", txtMarca.Text);
                cmd.Parameters.AddWithValue("@modelo", txtModelo.Text);
                cmd.Parameters.AddWithValue("@ano", txtAno.Text);
                cmd.Parameters.AddWithValue("@cor", txtCor.Text);
                cmd.Parameters.AddWithValue("@km", txtQuilometragem.Text);
                cmd.Parameters.AddWithValue("@id", idSelecionado);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Veículo atualizado com sucesso!", "Sucesso");
            }

            LimparCampos();
            CarregarVeiculos("");
        }

        private void BtnNovo_Click(object sender, EventArgs e) => LimparCampos();

        private void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (idSelecionado == 0)
            {
                MessageBox.Show("Selecione um veículo na tabela primeiro!", "Atenção");
                return;
            }
            var confirm = MessageBox.Show("Deseja excluir este veículo?", "Confirmar", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                using var conn = Conexao.Abrir();
                string sql = "DELETE FROM Veiculos WHERE Id=@id";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", idSelecionado);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Veículo excluído!", "Sucesso");
                LimparCampos();
                CarregarVeiculos("");
            }
        }

        private void BtnPesquisar_Click(object sender, EventArgs e) => CarregarVeiculos(txtPesquisa.Text);

        private void LimparCampos()
        {
            idSelecionado = 0;
            cmbCliente.SelectedIndex = -1;
            txtPlaca.Clear();
            txtMarca.Clear();
            txtModelo.Clear();
            txtAno.Clear();
            txtCor.Clear();
            txtQuilometragem.Clear();
        }
    }
}