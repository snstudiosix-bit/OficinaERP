using System;
using System.Drawing;
using System.Windows.Forms;

namespace OficinaERP.Forms
{
    public class FrmPrincipal : Form
    {
        private Panel sidebar = null!;
        private Panel painelDireito = null!;
        private Panel topbar = null!;
        private Panel painelConteudo = null!;
        private Label lblPagina = null!;

        public FrmPrincipal()
        {
            Text = "Oficina ERP";
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);
            MinimumSize = new Size(1100, 750);
            Font = new Font("Segoe UI", 9);

            try { this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }

            ConstruirLayout();
            CarregarDashboardPrincipal();
        }

        private void ConstruirLayout()
        {
            // --- SIDEBAR ---
            sidebar = new Panel()
            {
                Dock = DockStyle.Left,
                Width = 240,
                BackColor = Color.FromArgb(20, 28, 48),
                Padding = new Padding(0, 20, 0, 0)
            };

            var lblEmoji = new Label() { Text = "🔧", Dock = DockStyle.Top, Height = 45, Font = new Font("Segoe UI Emoji", 24), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter };
            var lblTitulo = new Label() { Text = "OFICINA ERP", Dock = DockStyle.Top, Height = 24, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.FromArgb(64, 196, 255), TextAlign = ContentAlignment.MiddleCenter };
            var lblSubtitulo = new Label() { Text = "Gestão Automotiva", Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 8), ForeColor = Color.FromArgb(120, 140, 180), TextAlign = ContentAlignment.MiddleCenter };
            var divisor = new Panel() { Dock = DockStyle.Top, Height = 1, BackColor = Color.FromArgb(40, 55, 80), Margin = new Padding(20, 10, 20, 10) };

            sidebar.Controls.AddRange(new Control[] { divisor, lblSubtitulo, lblTitulo, lblEmoji });

            var menuItens = new (string icone, string texto, Action acao)[]
            {
                ("📊", "Dashboard",         () => CarregarDashboardPrincipal()),
                ("👤", "Clientes",          () => AbrirFormNoPainel(new FrmClientes(), "Clientes")),
                ("🚗", "Veículos",          () => AbrirFormNoPainel(new FrmVeiculos(), "Veículos")),
                ("🔧", "Ordens de Serviço", () => AbrirFormNoPainel(new FrmOrdemServico(), "Ordens de Serviço")),
                ("🖥️", "Caixa / PDV",      () => AbrirFormNoPainel(new FrmCaixa(), "Caixa / PDV")),
                ("📦", "Estoque",           () => AbrirFormNoPainel(new FrmEstoque(), "Estoque")),
                ("💰", "Financeiro",        () => AbrirFormNoPainel(new FrmFinanceiro(), "Financeiro")),
                ("📋", "Relatórios",        () => AbrirFormNoPainel(new FrmRelatorios(), "Relatórios")),
                ("🔑", "Usuários",          () => {
                    if (Program.UsuarioPerfil != "Admin") {
                        MessageBox.Show("Apenas administradores podem acessar!", "Negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    AbrirFormNoPainel(new FrmUsuarios(), "Usuários");
                }),
            };

            for (int i = menuItens.Length - 1; i >= 0; i--)
            {
                var item = menuItens[i];
                var btn = CriarBotaoMenu(item.icone, item.texto);
                btn.Click += (s, e) => item.acao();
                sidebar.Controls.Add(btn);
            }

            // --- RODAPÉ SIDEBAR ---
            var painelRodape = new Panel() { Dock = DockStyle.Bottom, Height = 110, Padding = new Padding(15, 0, 15, 10) };
            var lblUsuario = new Label() { Text = $"👤 {Program.UsuarioNome}", Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = Color.FromArgb(64, 196, 255), TextAlign = ContentAlignment.MiddleCenter };
            var lblPerfil = new Label() { Text = Program.UsuarioPerfil, Dock = DockStyle.Top, Height = 16, Font = new Font("Segoe UI", 7), ForeColor = Color.FromArgb(100, 130, 170), TextAlign = ContentAlignment.MiddleCenter };
            var btnSair = new Button() { Text = "⏻  Sair", Dock = DockStyle.Bottom, Height = 28, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White, Font = new Font("Segoe UI", 8, FontStyle.Bold), Cursor = Cursors.Hand };
            btnSair.FlatAppearance.BorderSize = 0;
            btnSair.Click += (s, e) => { if (MessageBox.Show("Sair?", "Sair", MessageBoxButtons.YesNo) == DialogResult.Yes) Application.Restart(); };
            var lblVersao = new Label() { Text = "v1.0  •  2026", Dock = DockStyle.Bottom, Height = 16, Font = new Font("Segoe UI", 7), ForeColor = Color.FromArgb(70, 90, 120), TextAlign = ContentAlignment.MiddleCenter };
            painelRodape.Controls.AddRange(new Control[] { lblVersao, btnSair, lblPerfil, lblUsuario });
            sidebar.Controls.Add(painelRodape);

            // --- PAINEL DIREITO ---
            painelDireito = new Panel() { Dock = DockStyle.Fill, BackColor = Color.FromArgb(245, 247, 250) };

            // TOPBAR
            topbar = new Panel() { Dock = DockStyle.Top, Height = 60, BackColor = Color.White, Padding = new Padding(25, 0, 25, 0) };
            lblPagina = new Label() { Text = "Dashboard", Dock = DockStyle.Left, Width = 400, Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = Color.FromArgb(30, 40, 60), TextAlign = ContentAlignment.MiddleLeft };
            var lblData = new Label() { Text = DateTime.Now.ToString("dddd, dd/MM/yyyy"), Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(120, 140, 160), TextAlign = ContentAlignment.MiddleRight };
            topbar.Controls.AddRange(new Control[] { lblData, lblPagina });
            painelDireito.Controls.Add(topbar);

            // 🌟 PAINEL DE CONTEÚDO (AJUSTADO)
            // Aumentei o Padding para 40 no topo para garantir que os campos de Nome não sumam
            painelConteudo = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 242, 245),
                AutoScroll = true,
                Padding = new Padding(30, 40, 30, 30) // Margens generosas (Esquerda, Topo, Direita, Baixo)
            };
            painelDireito.Controls.Add(painelConteudo);

            Controls.AddRange(new Control[] { painelDireito, sidebar });
        }

        private void AbrirFormNoPainel(Form frmFilho, string tituloPagina)
        {
            painelConteudo.Controls.Clear();
            lblPagina.Text = tituloPagina;

            frmFilho.TopLevel = false;
            frmFilho.FormBorderStyle = FormBorderStyle.None;

            // Forçamos o background igual ao painel para não parecer uma janela "colada"
            frmFilho.BackColor = painelConteudo.BackColor;
            frmFilho.Dock = DockStyle.Fill;

            painelConteudo.Controls.Add(frmFilho);
            frmFilho.Show();
        }

        private void CarregarDashboardPrincipal()
        {
            painelConteudo.Controls.Clear();
            lblPagina.Text = $"Bem-vindo, {Program.UsuarioNome}!";

            // Cards no topo
            var flowCards = new FlowLayoutPanel() { Dock = DockStyle.Top, Height = 130, Padding = new Padding(0, 10, 0, 0), AutoScroll = false };
            var cards = new (string t, string v, string i, Color c)[] {
                ("Clientes", CarregarCount("Clientes"), "👤", Color.FromArgb(52, 152, 219)),
                ("Ordens", CarregarCount("OrdensServico"), "🔧", Color.FromArgb(230, 126, 34)),
                ("Veículos", CarregarCount("Veiculos"), "🚗", Color.FromArgb(155, 89, 182)),
                ("Estoque", CarregarCount("Pecas"), "📦", Color.FromArgb(46, 204, 113))
            };
            foreach (var card in cards) flowCards.Controls.Add(CriarCard(card.t, card.v, card.i, card.c, 185, 100));
            painelConteudo.Controls.Add(flowCards);

            // Container inferior
            var painelInferior = new Panel() { Dock = DockStyle.Fill, Padding = new Padding(0, 30, 0, 0) };

            // Atalhos
            var pnlAtalhos = new Panel() { Dock = DockStyle.Left, Width = 380 };
            pnlAtalhos.Controls.Add(new Label() { Text = "Atalhos Rápidos", Location = new Point(0, 0), Size = new Size(200, 25), Font = new Font("Segoe UI", 11, FontStyle.Bold) });
            var atalhos = new (string t, string i, Color c, Action a)[] {
                ("Nova O.S.", "🔧", Color.FromArgb(230, 126, 34), () => AbrirFormNoPainel(new FrmOrdemServico(), "Ordens de Serviço")),
                ("Caixa/PDV", "🖥️", Color.FromArgb(41, 128, 185), () => AbrirFormNoPainel(new FrmCaixa(), "Caixa / PDV")),
                ("Novo Cliente", "👤", Color.FromArgb(52, 152, 219), () => AbrirFormNoPainel(new FrmClientes(), "Clientes")),
                ("Financeiro", "💰", Color.FromArgb(231, 76, 60), () => AbrirFormNoPainel(new FrmFinanceiro(), "Financeiro"))
            };
            int topo = 40;
            foreach (var at in atalhos)
            {
                var btn = CriarBotaoAtalho(at.i + "   " + at.t, 0, topo, 360, 50, at.c);
                btn.Click += (s, e) => at.a();
                pnlAtalhos.Controls.Add(btn);
                topo += 60;
            }

            // Info Sistema
            var pnlInfo = new Panel() { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20) };
            pnlInfo.Paint += (s, e) => e.Graphics.DrawRectangle(new Pen(Color.FromArgb(220, 225, 235)), 0, 0, pnlInfo.Width - 1, pnlInfo.Height - 1);
            pnlInfo.Controls.Add(new Label() { Text = "📌  Status do Sistema", Dock = DockStyle.Top, Height = 35, Font = new Font("Segoe UI", 10, FontStyle.Bold) });
            string[] infos = { $"🖥️ Vendas: {CarregarCount("Vendas")}", "📁 Dados Locais", $"🔒 {Program.UsuarioNome}", $"📅 {DateTime.Now:dd/MM/yyyy}", "🗄️ Database OK", "✅ Online" };
            foreach (var info in infos) pnlInfo.Controls.Add(new Label() { Text = info, Dock = DockStyle.Top, Height = 30, TextAlign = ContentAlignment.MiddleLeft });

            painelInferior.Controls.AddRange(new Control[] { pnlInfo, pnlAtalhos });
            painelConteudo.Controls.Add(painelInferior);
        }

        // --- HELPERS REUTILIZÁVEIS ---
        private string CarregarCount(string t) { try { using var conn = Database.Conexao.Abrir(); return new Microsoft.Data.Sqlite.SqliteCommand($"SELECT COUNT(*) FROM {t}", conn).ExecuteScalar()?.ToString() ?? "0"; } catch { return "0"; } }

        private Panel CriarCard(string t, string v, string i, Color c, int w, int h)
        {
            var p = new Panel() { Size = new Size(w, h), BackColor = c, Margin = new Padding(0, 0, 20, 0) };
            p.Controls.Add(new Label() { Text = i, Dock = DockStyle.Top, Height = 35, Font = new Font("Segoe UI Emoji", 16), ForeColor = Color.FromArgb(100, 255, 255, 255), TextAlign = ContentAlignment.BottomCenter });
            p.Controls.Add(new Label() { Text = v, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter });
            p.Controls.Add(new Label() { Text = t, Dock = DockStyle.Bottom, Height = 25, ForeColor = Color.White, TextAlign = ContentAlignment.TopCenter, Font = new Font("Segoe UI", 8) });
            return p;
        }

        private Button CriarBotaoMenu(string i, string t)
        {
            var b = new Button() { Text = $"    {i}    {t}", Dock = DockStyle.Top, Height = 45, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(20, 28, 48), ForeColor = Color.FromArgb(180, 200, 230), Font = new Font("Segoe UI", 10), TextAlign = ContentAlignment.MiddleLeft, Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0; b.FlatAppearance.MouseOverBackColor = Color.FromArgb(35, 50, 80);
            return b;
        }

        private Button CriarBotaoAtalho(string t, int l, int tp, int w, int h, Color c)
        {
            var b = new Button() { Text = t, Location = new Point(l, tp), Size = new Size(w, h), FlatStyle = FlatStyle.Flat, BackColor = Color.White, ForeColor = c, Font = new Font("Segoe UI", 10, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft, Cursor = Cursors.Hand, Padding = new Padding(15, 0, 0, 0) };
            b.FlatAppearance.BorderColor = Color.FromArgb(220, 225, 235);
            return b;
        }
    }
}