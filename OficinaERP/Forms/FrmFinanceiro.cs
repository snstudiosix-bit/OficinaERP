using Microsoft.Data.Sqlite;
using OficinaERP.Database;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OficinaERP.Forms
{
    public class FrmFinanceiro : Form
    {
        private ComboBox cmbTipo = new ComboBox();
        private ComboBox cmbStatus = new ComboBox();
        private TextBox txtDescricao = new TextBox();
        private TextBox txtValor = new TextBox();
        private DateTimePicker dtpVencimento = new DateTimePicker();
        private DateTimePicker dtpPagamento = new DateTimePicker();
        private CheckBox chkPago = new CheckBox();
        private TextBox txtObservacao = new TextBox();
        private TextBox txtPesquisa = new TextBox();
        private ComboBox cmbFiltroTipo = new ComboBox();
        private ComboBox cmbFiltroStatus = new ComboBox();
        private Button btnSalvar = new Button();
        private Button btnNovo = new Button();
        private Button btnExcluir = new Button();
        private Button btnPesquisar = new Button();
        private Button btnMarcarPago = new Button();
        private DataGridView grid = new DataGridView();

        // Cards
        private Label lblCardReceber = new Label();
        private Label lblCardPagar = new Label();
        private Label lblCardSaldo = new Label();
        private Label lblCardVencidos = new Label();

        private int idSelecionado = 0;

        public FrmFinanceiro()
        {
            Text = "Financeiro";
            Width = 1050;
            Height = 720;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9);

            // ── TOPBAR ─────────────────────────────────────────────
            var topbar = new Panel()
            {
                Left = 0,
                Top = 0,
                Width = 1050,
                Height = 55,
                BackColor = Color.FromArgb(20, 28, 48)
            };
            topbar.Controls.Add(new Label()
            {
                Text = "💰  FINANCEIRO  —  Contas a Receber e a Pagar",
                Left = 20,
                Top = 0,
                Width = 700,
                Height = 55,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 196, 255),
                TextAlign = ContentAlignment.MiddleLeft
            });

            // ── CARDS DE RESUMO ────────────────────────────────────
            var painelCards = new Panel()
            {
                Left = 0,
                Top = 55,
                Width = 1050,
                Height = 100,
                BackColor = Color.FromArgb(245, 247, 250)
            };

            var cardReceber = CriarCard("A Receber", "R$ 0,00", "📈",
                Color.FromArgb(46, 204, 113), 10, 10, 235, 80, lblCardReceber);
            var cardPagar = CriarCard("A Pagar", "R$ 0,00", "📉",
                Color.FromArgb(231, 76, 60), 255, 10, 235, 80, lblCardPagar);
            var cardSaldo = CriarCard("Saldo", "R$ 0,00", "💼",
                Color.FromArgb(52, 152, 219), 500, 10, 235, 80, lblCardSaldo);
            var cardVencidos = CriarCard("Vencidos", "R$ 0,00", "⚠️",
                Color.FromArgb(230, 126, 34), 745, 10, 235, 80, lblCardVencidos);

            painelCards.Controls.AddRange(new Control[] { cardReceber, cardPagar, cardSaldo, cardVencidos });

            // ── PAINEL FORMULÁRIO (ESQUERDO) ───────────────────────
            var painelForm = new Panel()
            {
                Left = 10,
                Top = 165,
                Width = 340,
                Height = 510,
                BackColor = Color.White
            };
            painelForm.Paint += BorderPaint;

            painelForm.Controls.Add(new Label()
            {
                Text = "📝  NOVO LANÇAMENTO",
                Left = 15,
                Top = 15,
                Width = 310,
                Height = 22,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 28, 48)
            });

            var sep0 = new Panel() { Left = 15, Top = 42, Width = 310, Height = 1, BackColor = Color.FromArgb(220, 225, 235) };
            painelForm.Controls.Add(sep0);

            // Tipo
            AdicionarLabel(painelForm, "Tipo:", 15, 55);
            cmbTipo.Left = 15; cmbTipo.Top = 73; cmbTipo.Width = 150;
            cmbTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTipo.Items.AddRange(new string[] { "Receber", "Pagar" });
            cmbTipo.SelectedIndex = 0;

            // Status
            AdicionarLabel(painelForm, "Status:", 175, 55);
            cmbStatus.Left = 175; cmbStatus.Top = 73; cmbStatus.Width = 150;
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Items.AddRange(new string[] { "Pendente", "Pago", "Cancelado" });
            cmbStatus.SelectedIndex = 0;

            // Descrição
            AdicionarLabel(painelForm, "Descrição:", 15, 105);
            txtDescricao.Left = 15; txtDescricao.Top = 123; txtDescricao.Width = 310;

            // Valor
            AdicionarLabel(painelForm, "Valor R$:", 15, 150);
            txtValor.Left = 15; txtValor.Top = 168; txtValor.Width = 150;
            txtValor.Leave += TxtValor_Leave;

            // Vencimento
            AdicionarLabel(painelForm, "Vencimento:", 15, 200);
            dtpVencimento.Left = 15; dtpVencimento.Top = 218; dtpVencimento.Width = 180;
            dtpVencimento.Format = DateTimePickerFormat.Short;

            // Pago
            chkPago.Text = "✔  Marcar como Pago/Recebido";
            chkPago.Left = 15; chkPago.Top = 258; chkPago.Width = 310;
            chkPago.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            chkPago.ForeColor = Color.FromArgb(46, 204, 113);
            chkPago.CheckedChanged += ChkPago_CheckedChanged;

            AdicionarLabel(painelForm, "Data Pagamento:", 15, 285);
            dtpPagamento.Left = 15; dtpPagamento.Top = 303; dtpPagamento.Width = 180;
            dtpPagamento.Format = DateTimePickerFormat.Short;
            dtpPagamento.Enabled = false;

            // Observação
            AdicionarLabel(painelForm, "Observação:", 15, 335);
            txtObservacao.Left = 15; txtObservacao.Top = 353; txtObservacao.Width = 310;
            txtObservacao.Height = 50; txtObservacao.Multiline = true;

            // Botões
            btnSalvar.Text = "💾  Salvar";
            btnSalvar.Left = 15; btnSalvar.Top = 418; btnSalvar.Width = 140; btnSalvar.Height = 36;
            EstilizarBotao(btnSalvar, Color.FromArgb(52, 152, 219));
            btnSalvar.Click += BtnSalvar_Click;

            btnNovo.Text = "➕  Novo";
            btnNovo.Left = 165; btnNovo.Top = 418; btnNovo.Width = 80; btnNovo.Height = 36;
            EstilizarBotao(btnNovo, Color.FromArgb(46, 204, 113));
            btnNovo.Click += BtnNovo_Click;

            btnExcluir.Text = "🗑️";
            btnExcluir.Left = 255; btnExcluir.Top = 418; btnExcluir.Width = 70; btnExcluir.Height = 36;
            EstilizarBotao(btnExcluir, Color.FromArgb(231, 76, 60));
            btnExcluir.Click += BtnExcluir_Click;

            btnMarcarPago.Text = "✅  Quitar";
            btnMarcarPago.Left = 15; btnMarcarPago.Top = 462; btnMarcarPago.Width = 310; btnMarcarPago.Height = 36;
            EstilizarBotao(btnMarcarPago, Color.FromArgb(39, 174, 96));
            btnMarcarPago.Click += BtnMarcarPago_Click;

            painelForm.Controls.AddRange(new Control[] {
                cmbTipo, cmbStatus, txtDescricao, txtValor,
                dtpVencimento, chkPago, dtpPagamento,
                txtObservacao, btnSalvar, btnNovo, btnExcluir, btnMarcarPago
            });

            // ── PAINEL GRID (DIREITO) ──────────────────────────────
            var painelGrid = new Panel()
            {
                Left = 360,
                Top = 165,
                Width = 670,
                Height = 510,
                BackColor = Color.White
            };
            painelGrid.Paint += BorderPaint;

            painelGrid.Controls.Add(new Label()
            {
                Text = "📋  LANÇAMENTOS",
                Left = 15,
                Top = 15,
                Width = 300,
                Height = 22,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 28, 48)
            });

            // Filtros
            AdicionarLabel(painelGrid, "Buscar:", 15, 45);
            txtPesquisa.Left = 70; txtPesquisa.Top = 43; txtPesquisa.Width = 180;
            txtPesquisa.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) CarregarLancamentos(); };

            AdicionarLabel(painelGrid, "Tipo:", 260, 45);
            cmbFiltroTipo.Left = 295; cmbFiltroTipo.Top = 43; cmbFiltroTipo.Width = 100;
            cmbFiltroTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFiltroTipo.Items.AddRange(new string[] { "Todos", "Receber", "Pagar" });
            cmbFiltroTipo.SelectedIndex = 0;
            cmbFiltroTipo.SelectedIndexChanged += (s, e) => CarregarLancamentos();

            AdicionarLabel(painelGrid, "Status:", 405, 45);
            cmbFiltroStatus.Left = 450; cmbFiltroStatus.Top = 43; cmbFiltroStatus.Width = 110;
            cmbFiltroStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFiltroStatus.Items.AddRange(new string[] { "Todos", "Pendente", "Pago", "Cancelado" });
            cmbFiltroStatus.SelectedIndex = 0;
            cmbFiltroStatus.SelectedIndexChanged += (s, e) => CarregarLancamentos();

            btnPesquisar.Text = "🔍";
            btnPesquisar.Left = 570; btnPesquisar.Top = 41; btnPesquisar.Width = 80; btnPesquisar.Height = 28;
            EstilizarBotao(btnPesquisar, Color.FromArgb(52, 73, 94));
            btnPesquisar.Click += (s, e) => CarregarLancamentos();

            var sep1 = new Panel() { Left = 15, Top = 78, Width = 635, Height = 1, BackColor = Color.FromArgb(220, 225, 235) };

            // Grid
            grid.Left = 15; grid.Top = 85;
            grid.Width = 635; grid.Height = 395;
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
            grid.RowsDefaultCellStyle.BackColor = Color.White;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 253);
            grid.RowTemplate.Height = 32;
            grid.CellClick += Grid_CellClick;

            painelGrid.Controls.AddRange(new Control[] {
                txtPesquisa, cmbFiltroTipo, cmbFiltroStatus, btnPesquisar, sep1, grid
            });
            AdicionarLabel(painelGrid, "Buscar:", 15, 45);
            AdicionarLabel(painelGrid, "Tipo:", 260, 45);
            AdicionarLabel(painelGrid, "Status:", 405, 45);

            Controls.AddRange(new Control[] { topbar, painelCards, painelForm, painelGrid });

            CarregarLancamentos();
        }

        // ── HELPERS DE UI ──────────────────────────────────────────

        private Panel CriarCard(string titulo, string valorInicial, string icone,
            Color cor, int left, int top, int width, int height, Label lblValor)
        {
            var card = new Panel()
            {
                Left = left,
                Top = top,
                Width = width,
                Height = height,
                BackColor = cor
            };

            card.Controls.Add(new Label()
            {
                Text = icone,
                Left = 10,
                Top = 8,
                Width = 40,
                Height = 35,
                Font = new Font("Segoe UI Emoji", 16),
                ForeColor = Color.FromArgb(255, 255, 255, 100),
                TextAlign = ContentAlignment.MiddleCenter
            });

            card.Controls.Add(new Label()
            {
                Text = titulo,
                Left = 55,
                Top = 8,
                Width = 175,
                Height = 20,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(220, 235, 255)
            });

            lblValor.Text = valorInicial;
            lblValor.Left = 55; lblValor.Top = 28;
            lblValor.Width = 175; lblValor.Height = 30;
            lblValor.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblValor.ForeColor = Color.White;
            card.Controls.Add(lblValor);

            return card;
        }

        private void AdicionarLabel(Panel painel, string texto, int left, int top)
        {
            painel.Controls.Add(new Label()
            {
                Text = texto,
                Left = left,
                Top = top,
                Width = 140,
                Height = 18,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(100, 120, 150)
            });
        }

        private void EstilizarBotao(Button btn, Color cor)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = cor;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
        }

        private void BorderPaint(object sender, PaintEventArgs e)
        {
            var p = sender as Panel;
            if (p == null) return;
            e.Graphics.DrawRectangle(new Pen(Color.FromArgb(220, 225, 235)), 0, 0, p.Width - 1, p.Height - 1);
        }

        private void TxtValor_Leave(object sender, EventArgs e)
        {
            if (double.TryParse(txtValor.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double val))
                txtValor.Text = val.ToString("F2");
        }

        // ── LÓGICA ────────────────────────────────────────────────

        private void ChkPago_CheckedChanged(object sender, EventArgs e)
        {
            dtpPagamento.Enabled = chkPago.Checked;
            if (chkPago.Checked)
            {
                dtpPagamento.Value = DateTime.Now;
                cmbStatus.SelectedItem = "Pago";
            }
            else
            {
                cmbStatus.SelectedItem = "Pendente";
            }
        }

        private void CarregarLancamentos()
        {
            grid.Rows.Clear();
            grid.Columns.Clear();
            grid.Columns.Add("Id", "ID");
            grid.Columns.Add("Tipo", "Tipo");
            grid.Columns.Add("Descricao", "Descrição");
            grid.Columns.Add("Valor", "Valor R$");
            grid.Columns.Add("Vencimento", "Vencimento");
            grid.Columns.Add("Pagamento", "Pagamento");
            grid.Columns.Add("Status", "Status");
            grid.Columns["Id"].Visible = false;
            grid.Columns["Tipo"].Width = 70;
            grid.Columns["Status"].Width = 80;

            double totalReceber = 0, totalPagar = 0, totalVencidos = 0;
            string filtro = txtPesquisa.Text;
            string filtroTipo = cmbFiltroTipo.SelectedItem?.ToString() ?? "Todos";
            string filtroStatus = cmbFiltroStatus.SelectedItem?.ToString() ?? "Todos";

            using var conn = Conexao.Abrir();
            string sql = @"SELECT * FROM Financeiro
                           WHERE (Descricao LIKE @filtro OR Tipo LIKE @filtro)
                           ORDER BY DataVencimento DESC";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@filtro", $"%{filtro}%");
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                double valor = Convert.ToDouble(reader["Valor"]);
                string tipo = reader["Tipo"].ToString() ?? "";
                string status = reader["Status"].ToString() ?? "";
                string venc = reader["DataVencimento"].ToString() ?? "";

                // Aplicar filtros
                if (filtroTipo != "Todos" && tipo != filtroTipo) continue;
                if (filtroStatus != "Todos" && status != filtroStatus) continue;

                int rowIndex = grid.Rows.Add(
                    reader["Id"].ToString(),
                    tipo,
                    reader["Descricao"].ToString(),
                    $"R$ {valor:F2}",
                    venc,
                    reader["DataPagamento"].ToString(),
                    status
                );

                // Cores por status
                var row = grid.Rows[rowIndex];
                if (status == "Pago")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(232, 250, 240);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(39, 174, 96);
                }
                else if (status == "Cancelado")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
                    row.DefaultCellStyle.ForeColor = Color.Gray;
                }
                else if (status == "Pendente" && tipo == "Pagar")
                {
                    // Verifica se está vencido
                    if (DateTime.TryParse(venc, out DateTime dtVenc) && dtVenc < DateTime.Today)
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 235);
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(192, 57, 43);
                        totalVencidos += valor;
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 245, 230);
                        row.DefaultCellStyle.ForeColor = Color.FromArgb(150, 80, 0);
                    }
                }
                else if (status == "Pendente" && tipo == "Receber")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(235, 245, 255);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(41, 128, 185);
                }

                if (status != "Cancelado")
                {
                    if (tipo == "Receber") totalReceber += valor;
                    else totalPagar += valor;
                }
            }

            // Atualizar cards
            lblCardReceber.Text = $"R$ {totalReceber:F2}";
            lblCardPagar.Text = $"R$ {totalPagar:F2}";
            double saldo = totalReceber - totalPagar;
            lblCardSaldo.Text = $"R$ {saldo:F2}";
            lblCardVencidos.Text = $"R$ {totalVencidos:F2}";
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = grid.Rows[e.RowIndex];
            idSelecionado = int.Parse(row.Cells["Id"].Value?.ToString() ?? "0");

            using var conn = Conexao.Abrir();
            string sql = "SELECT * FROM Financeiro WHERE Id = @id";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", idSelecionado);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                cmbTipo.SelectedItem = reader["Tipo"].ToString();
                txtDescricao.Text = reader["Descricao"].ToString();
                txtValor.Text = Convert.ToDouble(reader["Valor"]).ToString("F2");
                cmbStatus.SelectedItem = reader["Status"].ToString();
                txtObservacao.Text = reader["Observacao"].ToString();

                if (DateTime.TryParse(reader["DataVencimento"].ToString(), out DateTime venc))
                    dtpVencimento.Value = venc;

                string pgto = reader["DataPagamento"].ToString() ?? "";
                if (!string.IsNullOrEmpty(pgto) && DateTime.TryParse(pgto, out DateTime dtPgto))
                {
                    chkPago.Checked = true;
                    dtpPagamento.Value = dtPgto;
                }
                else
                {
                    chkPago.Checked = false;
                }
            }
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDescricao.Text))
            {
                MessageBox.Show("A descrição é obrigatória!", "Atenção"); return;
            }
            if (!double.TryParse(txtValor.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double valor))
            {
                MessageBox.Show("Informe um valor válido!", "Atenção"); return;
            }

            string dataPgto = chkPago.Checked ? dtpPagamento.Value.ToString("yyyy-MM-dd") : "";

            using var conn = Conexao.Abrir();

            if (idSelecionado == 0)
            {
                string sql = @"INSERT INTO Financeiro 
                    (Tipo, Descricao, Valor, DataVencimento, DataPagamento, Status, Observacao)
                    VALUES (@tipo, @desc, @valor, @venc, @pgto, @status, @obs)";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@tipo", cmbTipo.SelectedItem?.ToString());
                cmd.Parameters.AddWithValue("@desc", txtDescricao.Text);
                cmd.Parameters.AddWithValue("@valor", valor);
                cmd.Parameters.AddWithValue("@venc", dtpVencimento.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@pgto", dataPgto);
                cmd.Parameters.AddWithValue("@status", cmbStatus.SelectedItem?.ToString());
                cmd.Parameters.AddWithValue("@obs", txtObservacao.Text);
                cmd.ExecuteNonQuery();
                MessageBox.Show("✅ Lançamento registrado!", "Sucesso");
            }
            else
            {
                string sql = @"UPDATE Financeiro SET Tipo=@tipo, Descricao=@desc, Valor=@valor,
                    DataVencimento=@venc, DataPagamento=@pgto, Status=@status, Observacao=@obs
                    WHERE Id=@id";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@tipo", cmbTipo.SelectedItem?.ToString());
                cmd.Parameters.AddWithValue("@desc", txtDescricao.Text);
                cmd.Parameters.AddWithValue("@valor", valor);
                cmd.Parameters.AddWithValue("@venc", dtpVencimento.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@pgto", dataPgto);
                cmd.Parameters.AddWithValue("@status", cmbStatus.SelectedItem?.ToString());
                cmd.Parameters.AddWithValue("@obs", txtObservacao.Text);
                cmd.Parameters.AddWithValue("@id", idSelecionado);
                cmd.ExecuteNonQuery();
                MessageBox.Show("✅ Lançamento atualizado!", "Sucesso");
            }

            LimparCampos();
            CarregarLancamentos();
        }

        private void BtnMarcarPago_Click(object sender, EventArgs e)
        {
            if (idSelecionado == 0)
            {
                MessageBox.Show("Selecione um lançamento primeiro!", "Atenção"); return;
            }

            using var conn = Conexao.Abrir();
            string sql = @"UPDATE Financeiro SET Status='Pago', DataPagamento=@pgto WHERE Id=@id";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@pgto", DateTime.Now.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@id", idSelecionado);
            cmd.ExecuteNonQuery();

            MessageBox.Show("✅ Lançamento quitado!", "Sucesso");
            LimparCampos();
            CarregarLancamentos();
        }

        private void BtnNovo_Click(object sender, EventArgs e) => LimparCampos();

        private void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (idSelecionado == 0)
            {
                MessageBox.Show("Selecione um lançamento primeiro!", "Atenção"); return;
            }
            if (MessageBox.Show("Deseja excluir este lançamento?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                using var conn = Conexao.Abrir();
                string sql = "DELETE FROM Financeiro WHERE Id=@id";
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", idSelecionado);
                cmd.ExecuteNonQuery();
                MessageBox.Show("🗑️ Lançamento excluído!", "Sucesso");
                LimparCampos();
                CarregarLancamentos();
            }
        }

        private void LimparCampos()
        {
            idSelecionado = 0;
            cmbTipo.SelectedIndex = 0;
            txtDescricao.Clear();
            txtValor.Clear();
            dtpVencimento.Value = DateTime.Now;
            chkPago.Checked = false;
            dtpPagamento.Enabled = false;
            cmbStatus.SelectedIndex = 0;
            txtObservacao.Clear();
        }
    }
}