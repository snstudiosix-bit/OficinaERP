using Microsoft.Data.Sqlite;
using OficinaERP.Database;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OficinaERP.Forms
{
    public class FrmCaixa : Form
    {
        private TextBox txtDescricao = new TextBox();
        private TextBox txtQtd = new TextBox();
        private TextBox txtValorUnit = new TextBox();
        private ComboBox cmbTipo = new ComboBox();
        private Button btnAdicionarItem = new Button();
        private ComboBox cmbCliente = new ComboBox();
        private ComboBox cmbOS = new ComboBox();
        private ComboBox cmbPagamento = new ComboBox();
        private TextBox txtDesconto = new TextBox();
        private TextBox txtObservacoes = new TextBox();
        private DataGridView grid = new DataGridView();
        private Label lblSubtotal = new Label();
        private Label lblDesconto = new Label();
        private Label lblTotal = new Label();
        private Label lblTroco = new Label();
        private TextBox txtPago = new TextBox();
        private Button btnConfirmar = new Button();
        private Button btnLimpar = new Button();
        private Button btnRemoverItem = new Button();

        private List<(string descricao, double qtd, double valorUnit, double valorTotal, string tipo)> itens = new();

        public FrmCaixa()
        {
            Text = "Caixa / PDV";
            Width = 920;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 9);

            // ── TOPBAR ─────────────────────────────────────────────
            var painelTopo = new Panel()
            {
                Left = 0,
                Top = 0,
                Width = 920,
                Height = 55,
                BackColor = Color.FromArgb(20, 28, 48)
            };
            painelTopo.Controls.Add(new Label()
            {
                Text = "🖥️  CAIXA / PDV  —  Oficina ERP",
                Left = 20,
                Top = 0,
                Width = 600,
                Height = 55,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 196, 255),
                TextAlign = ContentAlignment.MiddleLeft
            });

            // ── PAINEL ESQUERDO ────────────────────────────────────
            var painelEsq = new Panel()
            {
                Left = 10,
                Top = 65,
                Width = 560,
                Height = 560,
                BackColor = Color.White
            };
            painelEsq.Paint += BorderPaint;

            // ✅ Labels criados UMA vez como variáveis locais
            var lblCliente = new Label() { Text = "Cliente:", Left = 10, Top = 15, Width = 75, Height = 20, Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(80, 100, 130) };
            var lblOS = new Label() { Text = "OS:", Left = 370, Top = 15, Width = 30, Height = 20, Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(80, 100, 130) };
            var lblTipo = new Label() { Text = "Tipo:", Left = 10, Top = 62, Width = 45, Height = 20, Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(80, 100, 130) };
            var lblDescricao = new Label() { Text = "Descrição:", Left = 190, Top = 62, Width = 65, Height = 20, Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(80, 100, 130) };
            var lblQtd = new Label() { Text = "Qtd:", Left = 10, Top = 97, Width = 35, Height = 20, Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(80, 100, 130) };
            var lblVlrUnit = new Label() { Text = "Vlr Unit R$:", Left = 130, Top = 97, Width = 80, Height = 20, Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(80, 100, 130) };
            var lblObs = new Label() { Text = "Observações:", Left = 10, Top = 412, Width = 90, Height = 20, Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(80, 100, 130) };

            // Combos e inputs
            cmbCliente.Left = 90; cmbCliente.Top = 12; cmbCliente.Width = 270;
            cmbCliente.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCliente.SelectedIndexChanged += CmbCliente_Changed;

            cmbOS.Left = 405; cmbOS.Top = 12; cmbOS.Width = 140;
            cmbOS.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbOS.SelectedIndexChanged += CmbOS_Changed;

            var sep1 = new Panel() { Left = 10, Top = 48, Width = 535, Height = 1, BackColor = Color.FromArgb(220, 225, 235) };

            cmbTipo.Left = 60; cmbTipo.Top = 59; cmbTipo.Width = 120;
            cmbTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTipo.Items.AddRange(new string[] { "Peça", "Produto", "Mão de Obra", "Outros" });
            cmbTipo.SelectedIndex = 0;

            txtDescricao.Left = 258; txtDescricao.Top = 59; txtDescricao.Width = 290;

            txtQtd.Left = 60; txtQtd.Top = 94; txtQtd.Width = 60; txtQtd.Text = "1";

            txtValorUnit.Left = 215; txtValorUnit.Top = 94; txtValorUnit.Width = 90;
            txtValorUnit.Leave += TxtValor_Leave;

            btnAdicionarItem.Text = "➕ Adicionar";
            btnAdicionarItem.Left = 320; btnAdicionarItem.Top = 91;
            btnAdicionarItem.Width = 120; btnAdicionarItem.Height = 28;
            btnAdicionarItem.BackColor = Color.FromArgb(46, 204, 113);
            btnAdicionarItem.ForeColor = Color.White;
            btnAdicionarItem.FlatStyle = FlatStyle.Flat;
            btnAdicionarItem.FlatAppearance.BorderSize = 0;
            btnAdicionarItem.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnAdicionarItem.Cursor = Cursors.Hand;
            btnAdicionarItem.Click += BtnAdicionarItem_Click;

            btnRemoverItem.Text = "🗑️ Remover";
            btnRemoverItem.Left = 450; btnRemoverItem.Top = 91;
            btnRemoverItem.Width = 100; btnRemoverItem.Height = 28;
            btnRemoverItem.BackColor = Color.FromArgb(231, 76, 60);
            btnRemoverItem.ForeColor = Color.White;
            btnRemoverItem.FlatStyle = FlatStyle.Flat;
            btnRemoverItem.FlatAppearance.BorderSize = 0;
            btnRemoverItem.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnRemoverItem.Cursor = Cursors.Hand;
            btnRemoverItem.Click += BtnRemoverItem_Click;

            var sep2 = new Panel() { Left = 10, Top = 130, Width = 535, Height = 1, BackColor = Color.FromArgb(220, 225, 235) };

            grid.Left = 10; grid.Top = 138;
            grid.Width = 535; grid.Height = 265;
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
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);
            grid.Columns.Add("Tipo", "Tipo");
            grid.Columns.Add("Descricao", "Descrição");
            grid.Columns.Add("Qtd", "Qtd");
            grid.Columns.Add("ValorUnit", "Vlr Unit");
            grid.Columns.Add("ValorTotal", "Total");

            txtObservacoes.Left = 10; txtObservacoes.Top = 430;
            txtObservacoes.Width = 535; txtObservacoes.Height = 45;
            txtObservacoes.Multiline = true;

            // ✅ AddRange SEM duplicatas
            painelEsq.Controls.AddRange(new Control[] {
                lblCliente, cmbCliente,
                lblOS, cmbOS,
                sep1,
                lblTipo, cmbTipo,
                lblDescricao, txtDescricao,
                lblQtd, txtQtd,
                lblVlrUnit, txtValorUnit,
                btnAdicionarItem, btnRemoverItem,
                sep2, grid,
                lblObs, txtObservacoes
            });

            // ── PAINEL DIREITO ─────────────────────────────────────
            var painelDir = new Panel()
            {
                Left = 580,
                Top = 65,
                Width = 320,
                Height = 560,
                BackColor = Color.White
            };
            painelDir.Paint += BorderPaint;

            var lblPgtoTitulo = new Label()
            {
                Text = "💳  PAGAMENTO",
                Left = 10,
                Top = 15,
                Width = 300,
                Height = 25,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 28, 48)
            };

            var sep3 = new Panel() { Left = 10, Top = 45, Width = 295, Height = 1, BackColor = Color.FromArgb(220, 225, 235) };

            var lblFormaPgto = new Label() { Text = "Forma Pgto:", Left = 10, Top = 60, Width = 120, Height = 18, Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(80, 100, 130) };
            var lblDescontoL = new Label() { Text = "Desconto R$:", Left = 10, Top = 115, Width = 120, Height = 18, Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(80, 100, 130) };
            var lblValorPagoL = new Label() { Text = "Valor Pago R$:", Left = 10, Top = 310, Width = 120, Height = 18, Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(80, 100, 130) };

            cmbPagamento.Left = 10; cmbPagamento.Top = 78; cmbPagamento.Width = 295;
            cmbPagamento.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPagamento.Items.AddRange(new string[] { "Dinheiro", "Cartão de Débito", "Cartão de Crédito", "PIX" });
            cmbPagamento.SelectedIndex = 0;
            cmbPagamento.SelectedIndexChanged += AtualizarTotais;

            txtDesconto.Left = 10; txtDesconto.Top = 133; txtDesconto.Width = 140; txtDesconto.Text = "0,00";
            txtDesconto.Leave += (s, e) => { TxtValor_Leave(s, e); AtualizarTotais(s, e); };

            var sep4 = new Panel() { Left = 10, Top = 168, Width = 295, Height = 1, BackColor = Color.FromArgb(220, 225, 235) };

            lblSubtotal.Left = 10; lblSubtotal.Top = 180; lblSubtotal.Width = 295; lblSubtotal.Height = 25;
            lblSubtotal.Font = new Font("Segoe UI", 10); lblSubtotal.ForeColor = Color.FromArgb(70, 90, 110);
            lblSubtotal.Text = "Subtotal:  R$ 0,00";

            lblDesconto.Left = 10; lblDesconto.Top = 210; lblDesconto.Width = 295; lblDesconto.Height = 25;
            lblDesconto.Font = new Font("Segoe UI", 10); lblDesconto.ForeColor = Color.FromArgb(231, 76, 60);
            lblDesconto.Text = "Desconto:  R$ 0,00";

            var sep5 = new Panel() { Left = 10, Top = 242, Width = 295, Height = 2, BackColor = Color.FromArgb(20, 28, 48) };

            lblTotal.Left = 10; lblTotal.Top = 252; lblTotal.Width = 295; lblTotal.Height = 35;
            lblTotal.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTotal.ForeColor = Color.FromArgb(20, 28, 48);
            lblTotal.Text = "TOTAL:  R$ 0,00";

            var sep6 = new Panel() { Left = 10, Top = 295, Width = 295, Height = 1, BackColor = Color.FromArgb(220, 225, 235) };

            txtPago.Left = 10; txtPago.Top = 328; txtPago.Width = 140; txtPago.Text = "0,00";
            txtPago.Leave += (s, e) => { TxtValor_Leave(s, e); AtualizarTroco(); };

            lblTroco.Left = 10; lblTroco.Top = 362; lblTroco.Width = 295; lblTroco.Height = 30;
            lblTroco.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTroco.ForeColor = Color.FromArgb(46, 204, 113);
            lblTroco.Text = "Troco:  R$ 0,00";

            btnConfirmar.Text = "✅  CONFIRMAR VENDA";
            btnConfirmar.Left = 10; btnConfirmar.Top = 410; btnConfirmar.Width = 295; btnConfirmar.Height = 50;
            btnConfirmar.BackColor = Color.FromArgb(46, 204, 113);
            btnConfirmar.ForeColor = Color.White;
            btnConfirmar.FlatStyle = FlatStyle.Flat;
            btnConfirmar.FlatAppearance.BorderSize = 0;
            btnConfirmar.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnConfirmar.Cursor = Cursors.Hand;
            btnConfirmar.Click += BtnConfirmar_Click;

            btnLimpar.Text = "🗑️  Limpar Tudo";
            btnLimpar.Left = 10; btnLimpar.Top = 470; btnLimpar.Width = 295; btnLimpar.Height = 36;
            btnLimpar.BackColor = Color.FromArgb(245, 247, 250);
            btnLimpar.ForeColor = Color.FromArgb(100, 120, 140);
            btnLimpar.FlatStyle = FlatStyle.Flat;
            btnLimpar.FlatAppearance.BorderColor = Color.FromArgb(220, 225, 235);
            btnLimpar.Font = new Font("Segoe UI", 10);
            btnLimpar.Cursor = Cursors.Hand;
            btnLimpar.Click += (s, e) => LimparTudo();

            // ✅ AddRange SEM duplicatas
            painelDir.Controls.AddRange(new Control[] {
                lblPgtoTitulo, sep3,
                lblFormaPgto, cmbPagamento,
                lblDescontoL, txtDesconto,
                sep4,
                lblSubtotal, lblDesconto,
                sep5, lblTotal, sep6,
                lblValorPagoL, txtPago, lblTroco,
                btnConfirmar, btnLimpar
            });

            Controls.AddRange(new Control[] { painelTopo, painelEsq, painelDir });

            CarregarClientes();
        }

        // ── EVENTOS ────────────────────────────────────────────────

        private void CmbCliente_Changed(object sender, EventArgs e)
        {
            cmbOS.Items.Clear();
            cmbOS.Items.Add(new { Id = "0", Nome = "(Avulsa)" });
            if (cmbCliente.SelectedItem == null) return;
            dynamic cli = cmbCliente.SelectedItem;
            using var conn = Conexao.Abrir();
            var cmd = new SqliteCommand("SELECT Id FROM OrdensServico WHERE ClienteId=@cid ORDER BY Id DESC", conn);
            cmd.Parameters.AddWithValue("@cid", cli.Id);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                cmbOS.Items.Add(new { Id = r["Id"].ToString(), Nome = $"OS #{r["Id"]}" });
            cmbOS.DisplayMember = "Nome";
            cmbOS.SelectedIndex = 0;
        }

        private void CmbOS_Changed(object sender, EventArgs e)
        {
            if (cmbOS.SelectedItem == null) return;
            dynamic os = cmbOS.SelectedItem;
            if (os.Id == "0") return;
            using var conn = Conexao.Abrir();
            var cmd = new SqliteCommand("SELECT * FROM OrdensServico WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", os.Id);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return;
            double mdo = Convert.ToDouble(r["ValorMaoDeObra"]);
            if (mdo > 0)
                AdicionarItemLista("Mão de Obra", 1, mdo, "Mão de Obra");
        }

        private void BtnAdicionarItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDescricao.Text))
            {
                MessageBox.Show("Digite a descrição do item.", "Atenção"); return;
            }
            if (!double.TryParse(txtQtd.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double qtd) || qtd <= 0)
            {
                MessageBox.Show("Quantidade inválida.", "Atenção"); return;
            }
            if (!double.TryParse(txtValorUnit.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double vUnit) || vUnit <= 0)
            {
                MessageBox.Show("Valor unitário inválido.", "Atenção"); return;
            }
            AdicionarItemLista(txtDescricao.Text, qtd, vUnit, cmbTipo.SelectedItem?.ToString() ?? "Outros");
            txtDescricao.Clear(); txtQtd.Text = "1"; txtValorUnit.Clear();
            txtDescricao.Focus();
        }

        private void AdicionarItemLista(string descricao, double qtd, double valorUnit, string tipo)
        {
            double total = qtd * valorUnit;
            itens.Add((descricao, qtd, valorUnit, total, tipo));
            grid.Rows.Add(tipo, descricao, qtd.ToString("F2"), $"R$ {valorUnit:F2}", $"R$ {total:F2}");
            AtualizarTotais(null, null);
        }

        private void BtnRemoverItem_Click(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null || grid.CurrentRow.Index < 0) return;
            int idx = grid.CurrentRow.Index;
            itens.RemoveAt(idx);
            grid.Rows.RemoveAt(idx);
            AtualizarTotais(null, null);
        }

        private void AtualizarTotais(object sender, EventArgs e)
        {
            double subtotal = 0;
            foreach (var item in itens) subtotal += item.valorTotal;
            double.TryParse(txtDesconto.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double desconto);
            double total = Math.Max(subtotal - desconto, 0);
            lblSubtotal.Text = $"Subtotal:   R$ {subtotal:F2}";
            lblDesconto.Text = $"Desconto:  R$ {desconto:F2}";
            lblTotal.Text = $"TOTAL:  R$ {total:F2}";
            bool isDinheiro = cmbPagamento.SelectedItem?.ToString() == "Dinheiro";
            txtPago.Enabled = isDinheiro;
            lblTroco.Visible = isDinheiro;
            AtualizarTroco();
        }

        private void AtualizarTroco()
        {
            double.TryParse(lblTotal.Text.Replace("TOTAL:  R$ ", "").Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double total);
            double.TryParse(txtPago.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double pago);
            double troco = pago - total;
            lblTroco.Text = $"Troco:  R$ {Math.Max(troco, 0):F2}";
            lblTroco.ForeColor = troco >= 0 ? Color.FromArgb(46, 204, 113) : Color.FromArgb(231, 76, 60);
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            if (itens.Count == 0)
            {
                MessageBox.Show("Adicione pelo menos um item!", "Atenção"); return;
            }

            double.TryParse(lblTotal.Text.Replace("TOTAL:  R$ ", "").Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double total);
            double.TryParse(txtDesconto.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double desconto);

            dynamic? cli = cmbCliente.SelectedItem as dynamic;
            dynamic? os = cmbOS.SelectedItem as dynamic;
            string nomeCliente = cli != null ? (string)(cli.Nome ?? "Cliente avulso") : "Cliente avulso";
            string formaPgto = cmbPagamento.SelectedItem?.ToString() ?? "Dinheiro";

            using var conn = Conexao.Abrir();

            // 1. Salvar venda
            var cmd = new SqliteCommand(@"
                INSERT INTO Vendas (Data, ClienteId, OSId, FormaPagamento, Desconto, Total, Observacoes)
                VALUES (@data, @cid, @osid, @pgto, @desc, @total, @obs)", conn);
            cmd.Parameters.AddWithValue("@data", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            cmd.Parameters.AddWithValue("@cid", cli != null ? (object)cli.Id : DBNull.Value);
            cmd.Parameters.AddWithValue("@osid", os != null && os.Id != "0" ? (object)os.Id : DBNull.Value);
            cmd.Parameters.AddWithValue("@pgto", formaPgto);
            cmd.Parameters.AddWithValue("@desc", desconto);
            cmd.Parameters.AddWithValue("@total", total);
            cmd.Parameters.AddWithValue("@obs", txtObservacoes.Text);
            cmd.ExecuteNonQuery();

            // 2. ID da venda
            long vendaId = (long)(new SqliteCommand("SELECT last_insert_rowid()", conn).ExecuteScalar() ?? 0L);

            // 3. Salvar itens
            foreach (var item in itens)
            {
                var cmdItem = new SqliteCommand(@"
                    INSERT INTO VendaItens (VendaId, Descricao, Quantidade, ValorUnitario, ValorTotal, Tipo)
                    VALUES (@vid, @desc, @qtd, @unit, @total, @tipo)", conn);
                cmdItem.Parameters.AddWithValue("@vid", vendaId);
                cmdItem.Parameters.AddWithValue("@desc", item.descricao);
                cmdItem.Parameters.AddWithValue("@qtd", item.qtd);
                cmdItem.Parameters.AddWithValue("@unit", item.valorUnit);
                cmdItem.Parameters.AddWithValue("@total", item.valorTotal);
                cmdItem.Parameters.AddWithValue("@tipo", item.tipo);
                cmdItem.ExecuteNonQuery();
            }

            // 4. ✅ Registrar no Financeiro
            var cmdFin = new SqliteCommand(@"
                INSERT INTO Financeiro (Tipo, Descricao, Valor, DataVencimento, DataPagamento, Status, Observacao)
                VALUES (@tipo, @desc, @valor, @venc, @pgto, @status, @obs)", conn);
            cmdFin.Parameters.AddWithValue("@tipo", "Receber");
            cmdFin.Parameters.AddWithValue("@desc", $"Venda #{vendaId} - {nomeCliente}");
            cmdFin.Parameters.AddWithValue("@valor", total);
            cmdFin.Parameters.AddWithValue("@venc", DateTime.Now.ToString("yyyy-MM-dd"));
            cmdFin.Parameters.AddWithValue("@pgto", DateTime.Now.ToString("yyyy-MM-dd"));
            cmdFin.Parameters.AddWithValue("@status", "Pago");
            cmdFin.Parameters.AddWithValue("@obs", $"Pgto: {formaPgto} | {txtObservacoes.Text}");
            cmdFin.ExecuteNonQuery();

            MessageBox.Show(
                $"✅ Venda #{vendaId} confirmada!\n" +
                $"Total: R$ {total:F2}\n" +
                $"Forma: {formaPgto}\n\n" +
                $"💰 Registrado no Financeiro!",
                "Venda Realizada", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LimparTudo();
        }

        private void LimparTudo()
        {
            itens.Clear();
            grid.Rows.Clear();
            txtDescricao.Clear();
            txtQtd.Text = "1";
            txtValorUnit.Clear();
            txtDesconto.Text = "0,00";
            txtPago.Text = "0,00";
            txtObservacoes.Clear();
            cmbCliente.SelectedIndex = -1;
            cmbOS.Items.Clear();
            cmbPagamento.SelectedIndex = 0;
            lblSubtotal.Text = "Subtotal:   R$ 0,00";
            lblDesconto.Text = "Desconto:  R$ 0,00";
            lblTotal.Text = "TOTAL:  R$ 0,00";
            lblTroco.Text = "Troco:  R$ 0,00";
        }

        private void CarregarClientes()
        {
            cmbCliente.Items.Clear();
            using var conn = Conexao.Abrir();
            var cmd = new SqliteCommand("SELECT Id, Nome FROM Clientes ORDER BY Nome", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                cmbCliente.Items.Add(new { Id = r["Id"].ToString(), Nome = r["Nome"].ToString() });
            cmbCliente.DisplayMember = "Nome";
        }

        private void TxtValor_Leave(object sender, EventArgs e)
        {
            var txt = sender as TextBox;
            if (txt == null) return;
            if (double.TryParse(txt.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double val))
                txt.Text = val.ToString("F2");
        }

        private void BorderPaint(object sender, PaintEventArgs e)
        {
            var p = sender as Panel;
            if (p == null) return;
            e.Graphics.DrawRectangle(new Pen(Color.FromArgb(220, 225, 235)), 0, 0, p.Width - 1, p.Height - 1);
        }
    }
}