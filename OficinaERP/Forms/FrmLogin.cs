using Microsoft.Data.Sqlite;
using OficinaERP.Database;
using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace OficinaERP.Forms
{
    public class FrmLogin : Form
    {
        private TextBox txtLogin = new TextBox();
        private TextBox txtSenha = new TextBox();
        private Button btnEntrar = new Button();
        private Label lblErro = new Label();
        private CheckBox chkMostrarSenha = new CheckBox();

        public bool LoginOk { get; private set; } = false;
        public string NomeUsuario { get; private set; } = "";
        public string PerfilUsuario { get; private set; } = "";

        public FrmLogin()
        {
            Text = "Oficina ERP — Login";
            Width = 380;
            Height = 480;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(20, 28, 48);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // Logo
            var lblIcone = new Label()
            {
                Text = "🔧",
                Left = 0,
                Top = 40,
                Width = 380,
                Height = 60,
                Font = new Font("Segoe UI Emoji", 32),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblTitulo = new Label()
            {
                Text = "OFICINA ERP",
                Left = 0,
                Top = 105,
                Width = 380,
                Height = 28,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 196, 255),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblSub = new Label()
            {
                Text = "Faça login para continuar",
                Left = 0,
                Top = 135,
                Width = 380,
                Height = 20,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(120, 140, 180),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Painel do formulário
            var painelForm = new Panel()
            {
                Left = 30,
                Top = 170,
                Width = 320,
                Height = 230,
                BackColor = Color.FromArgb(30, 40, 60)
            };

            var lblLoginL = new Label()
            {
                Text = "Usuário:",
                Left = 15,
                Top = 20,
                Width = 290,
                Height = 18,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(150, 170, 210)
            };

            txtLogin.Left = 15; txtLogin.Top = 40;
            txtLogin.Width = 290; txtLogin.Height = 30;
            txtLogin.Font = new Font("Segoe UI", 10);
            txtLogin.BackColor = Color.FromArgb(20, 28, 48);
            txtLogin.ForeColor = Color.White;
            txtLogin.BorderStyle = BorderStyle.FixedSingle;

            var lblSenhaL = new Label()
            {
                Text = "Senha:",
                Left = 15,
                Top = 82,
                Width = 290,
                Height = 18,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(150, 170, 210)
            };

            txtSenha.Left = 15; txtSenha.Top = 102;
            txtSenha.Width = 290; txtSenha.Height = 30;
            txtSenha.Font = new Font("Segoe UI", 10);
            txtSenha.BackColor = Color.FromArgb(20, 28, 48);
            txtSenha.ForeColor = Color.White;
            txtSenha.BorderStyle = BorderStyle.FixedSingle;
            txtSenha.PasswordChar = '●';
            txtSenha.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) Entrar(); };

            chkMostrarSenha.Text = "Mostrar senha";
            chkMostrarSenha.Left = 15; chkMostrarSenha.Top = 140;
            chkMostrarSenha.Width = 150; chkMostrarSenha.Height = 20;
            chkMostrarSenha.Font = new Font("Segoe UI", 8);
            chkMostrarSenha.ForeColor = Color.FromArgb(120, 140, 180);
            chkMostrarSenha.BackColor = Color.FromArgb(30, 40, 60);
            chkMostrarSenha.CheckedChanged += (s, e) =>
                txtSenha.PasswordChar = chkMostrarSenha.Checked ? '\0' : '●';

            btnEntrar.Text = "ENTRAR";
            btnEntrar.Left = 15; btnEntrar.Top = 170;
            btnEntrar.Width = 290; btnEntrar.Height = 42;
            btnEntrar.BackColor = Color.FromArgb(52, 152, 219);
            btnEntrar.ForeColor = Color.White;
            btnEntrar.FlatStyle = FlatStyle.Flat;
            btnEntrar.FlatAppearance.BorderSize = 0;
            btnEntrar.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnEntrar.Cursor = Cursors.Hand;
            btnEntrar.Click += (s, e) => Entrar();

            painelForm.Controls.AddRange(new Control[] {
                lblLoginL, txtLogin,
                lblSenhaL, txtSenha,
                chkMostrarSenha, btnEntrar
            });

            lblErro.Left = 30; lblErro.Top = 410;
            lblErro.Width = 320; lblErro.Height = 22;
            lblErro.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblErro.ForeColor = Color.FromArgb(231, 76, 60);
            lblErro.TextAlign = ContentAlignment.MiddleCenter;

            var lblVersao = new Label()
            {
                Text = "v1.0  •  Oficina ERP © 2025",
                Left = 0,
                Top = 440,
                Width = 380,
                Height = 18,
                Font = new Font("Segoe UI", 7),
                ForeColor = Color.FromArgb(60, 80, 110),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Controls.AddRange(new Control[] {
                lblIcone, lblTitulo, lblSub,
                painelForm, lblErro, lblVersao
            });

            txtLogin.Focus();
        }

        private void Entrar()
        {
            if (string.IsNullOrWhiteSpace(txtLogin.Text) || string.IsNullOrWhiteSpace(txtSenha.Text))
            {
                lblErro.Text = "⚠️ Preencha usuário e senha!";
                return;
            }

            string senhaMd5 = GerarMd5(txtSenha.Text);

            try
            {
                using var conn = Conexao.Abrir();
                var cmd = new SqliteCommand(
                    "SELECT Nome, Perfil FROM Usuarios WHERE Login=@login AND Senha=@senha", conn);
                cmd.Parameters.AddWithValue("@login", txtLogin.Text.Trim());
                cmd.Parameters.AddWithValue("@senha", senhaMd5);
                using var r = cmd.ExecuteReader();

                if (r.Read())
                {
                    LoginOk = true;
                    NomeUsuario = r["Nome"].ToString() ?? "";
                    PerfilUsuario = r["Perfil"].ToString() ?? "";
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    lblErro.Text = "❌ Usuário ou senha incorretos!";
                    txtSenha.Clear();
                    txtSenha.Focus();
                }
            }
            catch
            {
                lblErro.Text = "❌ Erro ao conectar ao banco!";
            }
        }

        public static string GerarMd5(string texto)
        {
            using var md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(texto));
            var sb = new StringBuilder();
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}