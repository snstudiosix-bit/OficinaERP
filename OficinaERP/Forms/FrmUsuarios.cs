using Microsoft.Data.Sqlite;
using OficinaERP.Database;
using System.Drawing;
using System.Windows.Forms;

namespace OficinaERP.Forms
{
    public class FrmUsuarios : Form
    {
        private TextBox txtNome = new TextBox();
        private TextBox txtLogin = new TextBox();
        private TextBox txtSenha = new TextBox();
        private ComboBox cmbPerfil = new ComboBox();
        private Button btnSalvar = new Button();
        private Button btnNovo = new Button();
        private Button btnExcluir = new Button();
        private DataGridView grid = new DataGridView();
        private int idSelecionado = 0;

        public FrmUsuarios()
        {
            Text = "Gerenciar Usuários";
            Width = 620; Height = 500;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9);

            var topbar = new Panel()
            {
                Left = 0,
                Top = 0,
                Width = 620,
                Height = 50,
                BackColor = Color.FromArgb(20, 28, 48)
            };
            topbar.Controls.Add(new Label()
            {
                Text = "👤  GERENCIAR USUÁRIOS",
                Left = 15,
                Top = 0,
                Width = 500,
                Height = 50,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 196, 255),
                TextAlign = ContentAlignment.MiddleLeft
            });

            var painel = new Panel()
            {
                Left = 10,
                Top = 60,
                Width = 590,
                Height = 390,
                BackColor = Color.White
            };

            int col1 = 15, col2 = 110, larg = 200;

            AdicionarLabel(painel, "Nome:", col1, 20);
            txtNome.Left = col2; txtNome.Top = 18; txtNome.Width = larg + 100;

            AdicionarLabel(painel, "Login:", col1, 55);
            txtLogin.Left = col2; txtLogin.Top = 53; txtLogin.Width = larg;

            AdicionarLabel(painel, "Senha:", col1, 90);
            txtSenha.Left = col2; txtSenha.Top = 88; txtSenha.Width = larg;
            txtSenha.PasswordChar = '●';

            AdicionarLabel(painel, "Perfil:", col1, 125);
            cmbPerfil.Left = col2; cmbPerfil.Top = 123; cmbPerfil.Width = 150;
            cmbPerfil.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPerfil.Items.AddRange(new string[] { "Admin", "Operador" });
            cmbPerfil.SelectedIndex = 1;

            btnSalvar.Text = "💾 Salvar";
            btnSalvar.Left = col2; btnSalvar.Top = 163; btnSalvar.Width = 110; btnSalvar.Height = 32;
            EstilizarBotao(btnSalvar, Color.FromArgb(52, 152, 219));
            btnSalvar.Click += BtnSalvar_Click;

            btnNovo.Text = "➕ Novo";
            btnNovo.Left = col2 + 120; btnNovo.Top = 163; btnNovo.Width = 100; btnNovo.Height = 32;
            EstilizarBotao(btnNovo, Color.FromArgb(46, 204, 113));
            btnNovo.Click += (s, e) => LimparCampos();

            btnExcluir.Text = "🗑️ Excluir";
            btnExcluir.Left = col2 + 230; btnExcluir.Top = 163; btnExcluir.Width = 100; btnExcluir.Height = 32;
            EstilizarBotao(btnExcluir, Color.FromArgb(231, 76, 60));
            btnExcluir.Click += BtnExcluir_Click;

            grid.Left = 15; grid.Top = 210;
            grid.Width = 555; grid.Height = 160;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.RowHeadersVisible = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 28, 48);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            grid.EnableHeadersVisualStyles = false;
            grid.CellClick += Grid_CellClick;

            painel.Controls.AddRange(new Control[] {
                txtNome, txtLogin, txtSenha, cmbPerfil,
                btnSalvar, btnNovo, btnExcluir, grid
            });
            AdicionarLabel(painel, "Nome:", col1, 20);
            AdicionarLabel(painel, "Login:", col1, 55);
            AdicionarLabel(painel, "Senha:", col1, 90);
            AdicionarLabel(painel, "Perfil:", col1, 125);

            Controls.AddRange(new Control[] { topbar, painel });
            CarregarUsuarios();
        }

        private void CarregarUsuarios()
        {
            grid.Rows.Clear();
            grid.Columns.Clear();
            grid.Columns.Add("Id", "ID");
            grid.Columns.Add("Nome", "Nome");
            grid.Columns.Add("Login", "Login");
            grid.Columns.Add("Perfil", "Perfil");
            grid.Columns["Id"].Visible = false;

            using var conn = Conexao.Abrir();
            var cmd = new SqliteCommand("SELECT Id, Nome, Login, Perfil FROM Usuarios ORDER BY Nome", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                grid.Rows.Add(r["Id"], r["Nome"], r["Login"], r["Perfil"]);
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = grid.Rows[e.RowIndex];
            idSelecionado = int.Parse(row.Cells["Id"].Value?.ToString() ?? "0");
            txtNome.Text = row.Cells["Nome"].Value?.ToString();
            txtLogin.Text = row.Cells["Login"].Value?.ToString();
            txtSenha.Clear();
            cmbPerfil.SelectedItem = row.Cells["Perfil"].Value?.ToString();
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text) || string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                MessageBox.Show("Nome e login são obrigatórios!", "Atenção"); return;
            }

            using var conn = Conexao.Abrir();

            if (idSelecionado == 0)
            {
                if (string.IsNullOrWhiteSpace(txtSenha.Text))
                {
                    MessageBox.Show("Informe a senha!", "Atenção"); return;
                }
                var cmd = new SqliteCommand(
                    "INSERT INTO Usuarios (Nome, Login, Senha, Perfil) VALUES (@n, @l, @s, @p)", conn);
                cmd.Parameters.AddWithValue("@n", txtNome.Text);
                cmd.Parameters.AddWithValue("@l", txtLogin.Text);
                cmd.Parameters.AddWithValue("@s", FrmLogin.GerarMd5(txtSenha.Text));
                cmd.Parameters.AddWithValue("@p", cmbPerfil.SelectedItem?.ToString());
                cmd.ExecuteNonQuery();
                MessageBox.Show("✅ Usuário criado!", "Sucesso");
            }
            else
            {
                // Se senha em branco, não altera
                string novaSenha = string.IsNullOrWhiteSpace(txtSenha.Text)
                    ? "" : FrmLogin.GerarMd5(txtSenha.Text);

                string sql = string.IsNullOrWhiteSpace(txtSenha.Text)
                    ? "UPDATE Usuarios SET Nome=@n, Login=@l, Perfil=@p WHERE Id=@id"
                    : "UPDATE Usuarios SET Nome=@n, Login=@l, Senha=@s, Perfil=@p WHERE Id=@id";

                var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@n", txtNome.Text);
                cmd.Parameters.AddWithValue("@l", txtLogin.Text);
                if (!string.IsNullOrWhiteSpace(txtSenha.Text))
                    cmd.Parameters.AddWithValue("@s", novaSenha);
                cmd.Parameters.AddWithValue("@p", cmbPerfil.SelectedItem?.ToString());
                cmd.Parameters.AddWithValue("@id", idSelecionado);
                cmd.ExecuteNonQuery();
                MessageBox.Show("✅ Usuário atualizado!", "Sucesso");
            }

            LimparCampos();
            CarregarUsuarios();
        }

        private void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (idSelecionado == 0) { MessageBox.Show("Selecione um usuário!", "Atenção"); return; }
            if (MessageBox.Show("Excluir usuário?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using var conn = Conexao.Abrir();
                var cmd = new SqliteCommand("DELETE FROM Usuarios WHERE Id=@id", conn);
                cmd.Parameters.AddWithValue("@id", idSelecionado);
                cmd.ExecuteNonQuery();
                LimparCampos();
                CarregarUsuarios();
            }
        }

        private void LimparCampos()
        {
            idSelecionado = 0;
            txtNome.Clear(); txtLogin.Clear(); txtSenha.Clear();
            cmbPerfil.SelectedIndex = 1;
        }

        private void AdicionarLabel(Panel p, string texto, int left, int top) =>
            p.Controls.Add(new Label() { Text = texto, Left = left, Top = top, Width = 90, Height = 20, Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(80, 100, 130) });

        private void EstilizarBotao(Button btn, Color cor)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = cor;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
        }
    }
}