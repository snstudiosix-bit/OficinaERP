using Microsoft.Data.Sqlite;
using OficinaERP.Database;
using OficinaERP.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OficinaERP.Forms
{
    public class FrmClientes : Form
    {
        private TextBox txtNome = new TextBox();
        private TextBox txtTelefone = new TextBox();
        private TextBox txtCpfCnpj = new TextBox();
        private TextBox txtEndereco = new TextBox();
        private TextBox txtPesquisa = new TextBox();
        private Button btnSalvar = new Button();
        private Button btnNovo = new Button();
        private Button btnExcluir = new Button();
        private Button btnPesquisar = new Button();
        private DataGridView grid = new DataGridView();
        private int idSelecionado = 0;

        public FrmClientes()
        {
            Text = "Cadastro de Clientes";
            Width = 800;
            Height = 600;
            StartPosition = FormStartPosition.CenterScreen;

            // Labels e campos
            var lblNome = new Label() { Text = "Nome:", Left = 20, Top = 20, Width = 80 };
            txtNome.Left = 110; txtNome.Top = 18; txtNome.Width = 250;

            var lblTelefone = new Label() { Text = "Telefone:", Left = 20, Top = 55, Width = 80 };
            txtTelefone.Left = 110; txtTelefone.Top = 53; txtTelefone.Width = 250;

            var lblCpfCnpj = new Label() { Text = "CPF/CNPJ:", Left = 20, Top = 90, Width = 80 };
            txtCpfCnpj.Left = 110; txtCpfCnpj.Top = 88; txtCpfCnpj.Width = 250;

            var lblEndereco = new Label() { Text = "Endereço:", Left = 20, Top = 125, Width = 80 };
            txtEndereco.Left = 110; txtEndereco.Top = 123; txtEndereco.Width = 250;

            // Botões do formulário
            btnSalvar.Text = "Salvar";
            btnSalvar.Left = 110; btnSalvar.Top = 165; btnSalvar.Width = 100;
            btnSalvar.Click += BtnSalvar_Click;

            btnNovo.Text = "Novo";
            btnNovo.Left = 220; btnNovo.Top = 165; btnNovo.Width = 100;
            btnNovo.Click += BtnNovo_Click;

            btnExcluir.Text = "Excluir";
            btnExcluir.Left = 330; btnExcluir.Top = 165; btnExcluir.Width = 100;
            btnExcluir.Click += BtnExcluir_Click;

            // Pesquisa
            var lblPesquisa = new Label() { Text = "Pesquisar:", Left = 20, Top = 210, Width = 80 };
            txtPesquisa.Left = 110; txtPesquisa.Top = 208; txtPesquisa.Width = 200;

            btnPesquisar.Text = "Buscar";
            btnPesquisar.Left = 320; btnPesquisar.Top = 206; btnPesquisar.Width = 80;
            btnPesquisar.Click += BtnPesquisar_Click;

            // Grid
            grid.Left = 20; grid.Top = 245;
            grid.Width = 740; grid.Height = 300;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.CellClick += Grid_CellClick;

            Controls.AddRange(new Control[] {
                lblNome, txtNome,
                lblTelefone, txtTelefone,
                lblCpfCnpj, txtCpfCnpj,
                lblEndereco, txtEndereco,
                btnSalvar, btnNovo, btnExcluir,
                lblPesquisa, txtPesquisa, btnPesquisar,
                grid
            });

            CarregarClientes("");
        }

        private void CarregarClientes(string filtro)
        {
            grid.Rows.Clear();
            grid.Columns.Clear();
            grid.Columns.Add("Id", "ID");
            grid.Columns.Add("Nome", "Nome");
            grid.Columns.Add("Telefone", "Telefone");
            grid.Columns.Add("CpfCnpj", "CPF/CNPJ");
            grid.Columns.Add("Endereco", "Endereço");
            grid.Columns["Id"].Visible = false;

            using var conn = Conexao.Abrir();
            string sql = "SELECT Id, Nome, Telefone, CpfCnpj, Endereco FROM Clientes WHERE Nome LIKE @filtro";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@filtro", $"%{filtro}%");
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                grid.Rows.Add(
                    reader["Id"].ToString(),
                    reader["Nome"].ToString(),
                    reader["Telefone"].ToString(),
                    reader["CpfCnpj"].ToString(),
                    reader["Endereco"].ToString()
                );
            }
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = grid.Rows[e.RowIndex];
            idSelecionado = int.Parse(row.Cells["Id"].Value.ToString());
            txtNome.Text = row.Cells["Nome"].Value.ToString();
            txtTelefone.Text = row.Cells["Telefone"].Value.ToString();
            txtCpfCnpj.Text = row.Cells["CpfCnpj"].Value.ToString();
            txtEndereco.Text = row.Cells["Endereco"].Value.ToString();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("O nome é obrigatório!", "Atenção");
                return;
            }

            using var conn = Conexao.Abrir();

            if (idSelecionado == 0)
            {
                string sql = "INSERT INTO Clientes (Nome, Telefone, CpfCnpj, Endereco) VALUES (@nome, @tel, @cpf, @end)";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", txtNome.Text);
                cmd.Parameters.AddWithValue("@tel", txtTelefone.Text);
                cmd.Parameters.AddWithValue("@cpf", txtCpfCnpj.Text);
                cmd.Parameters.AddWithValue("@end", txtEndereco.Text);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Cliente cadastrado com sucesso!", "Sucesso");
            }
            else
            {
                string sql = "UPDATE Clientes SET Nome=@nome, Telefone=@tel, CpfCnpj=@cpf, Endereco=@end WHERE Id=@id";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome", txtNome.Text);
                cmd.Parameters.AddWithValue("@tel", txtTelefone.Text);
                cmd.Parameters.AddWithValue("@cpf", txtCpfCnpj.Text);
                cmd.Parameters.AddWithValue("@end", txtEndereco.Text);
                cmd.Parameters.AddWithValue("@id", idSelecionado);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Cliente atualizado com sucesso!", "Sucesso");
            }

            LimparCampos();
            CarregarClientes("");
        }

        private void BtnNovo_Click(object sender, EventArgs e)
        {
            LimparCampos();
        }

        private void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (idSelecionado == 0)
            {
                MessageBox.Show("Selecione um cliente na tabela primeiro!", "Atenção");
                return;
            }

            var confirm = MessageBox.Show("Deseja excluir este cliente?", "Confirmar", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                using var conn = Conexao.Abrir();
                string sql = "DELETE FROM Clientes WHERE Id=@id";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", idSelecionado);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Cliente excluído!", "Sucesso");
                LimparCampos();
                CarregarClientes("");
            }
        }

        private void BtnPesquisar_Click(object sender, EventArgs e)
        {
            CarregarClientes(txtPesquisa.Text);
        }

        private void LimparCampos()
        {
            idSelecionado = 0;
            txtNome.Clear();
            txtTelefone.Clear();
            txtCpfCnpj.Clear();
            txtEndereco.Clear();
        }
    }
}