// PdfHelper.cs
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.IO;

namespace OficinaERP.Relatorios
{
    public static class PdfHelper
    {
        public static void GerarOrdemServico(
            int numeroOS,
            string dataAbertura,
            string cliente,
            string placa,
            string marca,
            string modelo,
            string ano,
            string cor,
            string defeito,
            string diagnostico,
            string servicosExecutados,
            string pecasUtilizadas,
            string maodeObra,
            string valorTotal,
            string observacoes)
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = $"OS_{numeroOS:D3}";

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // ✅ CORRIGIDO - XFontStyle → XFontStyleEx
            XFont fonteTitulo = new XFont("Arial", 16, XFontStyleEx.Bold);
            XFont fonteSubtitulo = new XFont("Arial", 11, XFontStyleEx.Bold);
            XFont fonteNormal = new XFont("Arial", 10, XFontStyleEx.Regular);
            XFont fontePequena = new XFont("Arial", 9, XFontStyleEx.Regular);

            // ✅ CORRIGIDO - .Point para evitar aviso de obsoleto
            double largura = page.Width.Point;
            double y = 30;

            // ── CABEÇALHO ──────────────────────────────────────────
            gfx.DrawString("OFICINA MECÂNICA", fonteTitulo, XBrushes.DarkBlue,
                new XRect(0, y, largura, 30), XStringFormats.TopCenter);
            y += 22;

            gfx.DrawString("Sistema de Gestão - Ordem de Serviço", fonteNormal, XBrushes.Gray,
                new XRect(0, y, largura, 20), XStringFormats.TopCenter);
            y += 25;

            // Linha separadora
            gfx.DrawLine(XPens.DarkBlue, 40, y, largura - 40, y);
            y += 10;

            // ── NÚMERO DA OS E DATA ────────────────────────────────
            gfx.DrawString($"Ordem de Serviço Nº: {numeroOS:D3}", fonteSubtitulo, XBrushes.Black, 40, y);
            gfx.DrawString($"Data: {dataAbertura}", fonteNormal, XBrushes.Black,
                new XRect(0, y, largura - 40, 20), XStringFormats.TopRight);
            y += 25;

            // ── DADOS DO CLIENTE ───────────────────────────────────
            DesenharSecao(gfx, "DADOS DO CLIENTE", fonteSubtitulo, largura, ref y);

            DesenharCampo(gfx, "Cliente:", cliente, fonteNormal, 40, ref y);
            DesenharCampo(gfx, "Placa:", placa, fonteNormal, 40, ref y);
            DesenharCampo(gfx, "Veículo:", $"{marca} {modelo} - {ano} - {cor}", fonteNormal, 40, ref y);
            y += 5;

            // ── DEFEITO E DIAGNÓSTICO ──────────────────────────────
            DesenharSecao(gfx, "DEFEITO E DIAGNÓSTICO", fonteSubtitulo, largura, ref y);

            DesenharCampoMultilinha(gfx, "Defeito Informado:", defeito, fonteNormal, fontePequena, largura, ref y);
            DesenharCampoMultilinha(gfx, "Diagnóstico:", diagnostico, fonteNormal, fontePequena, largura, ref y);
            y += 5;

            // ── SERVIÇOS E PEÇAS ───────────────────────────────────
            DesenharSecao(gfx, "SERVIÇOS E PEÇAS", fonteSubtitulo, largura, ref y);

            DesenharCampoMultilinha(gfx, "Serviços Executados:", servicosExecutados, fonteNormal, fontePequena, largura, ref y);
            DesenharCampoMultilinha(gfx, "Peças Utilizadas:", pecasUtilizadas, fonteNormal, fontePequena, largura, ref y);
            y += 5;

            // ── VALORES ────────────────────────────────────────────
            DesenharSecao(gfx, "VALORES", fonteSubtitulo, largura, ref y);

            DesenharCampo(gfx, "Mão de Obra:", maodeObra, fonteNormal, 40, ref y);
            DesenharCampo(gfx, "Valor Total:", valorTotal, fonteNormal, 40, ref y);
            y += 5;

            // ── OBSERVAÇÕES ────────────────────────────────────────
            if (!string.IsNullOrWhiteSpace(observacoes))
            {
                DesenharSecao(gfx, "OBSERVAÇÕES", fonteSubtitulo, largura, ref y);
                DesenharCampoMultilinha(gfx, "", observacoes, fonteNormal, fontePequena, largura, ref y);
                y += 5;
            }

            // ── ASSINATURA ─────────────────────────────────────────
            y += 20;
            gfx.DrawLine(XPens.Black, 40, y, 250, y);
            y += 5;
            gfx.DrawString("Assinatura do Cliente", fontePequena, XBrushes.Gray, 40, y);

            gfx.DrawLine(XPens.Black, largura - 250, y - 5, largura - 40, y - 5);
            gfx.DrawString("Responsável Técnico", fontePequena, XBrushes.Gray, largura - 250, y);

            // ── RODAPÉ ─────────────────────────────────────────────
            y += 30;
            gfx.DrawLine(XPens.DarkBlue, 40, y, largura - 40, y);
            y += 8;
            gfx.DrawString($"Documento gerado em {DateTime.Now:dd/MM/yyyy HH:mm}", fontePequena, XBrushes.Gray,
                new XRect(0, y, largura, 15), XStringFormats.TopCenter);

            // ── SALVAR ─────────────────────────────────────────────
            string pasta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "OficinaERP", "OS");

            Directory.CreateDirectory(pasta);

            string arquivo = Path.Combine(pasta, $"OS_{numeroOS:D3}.pdf");
            document.Save(arquivo);

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = arquivo,
                UseShellExecute = true
            });
        }

        // ── HELPERS ────────────────────────────────────────────────

        private static void DesenharSecao(XGraphics gfx, string titulo, XFont fonte, double largura, ref double y)
        {
            gfx.DrawRectangle(XBrushes.DarkBlue, new XRect(40, y, largura - 80, 18));
            gfx.DrawString(titulo, fonte, XBrushes.White, 45, y + 3);
            y += 22;
        }

        private static void DesenharCampo(XGraphics gfx, string label, string valor, XFont fonte, double x, ref double y)
        {
            gfx.DrawString($"{label} {valor}", fonte, XBrushes.Black, x, y);
            y += 16;
        }

        private static void DesenharCampoMultilinha(XGraphics gfx, string label, string valor,
            XFont fontLabel, XFont fontValor, double largura, ref double y)
        {
            if (!string.IsNullOrWhiteSpace(label))
            {
                gfx.DrawString(label, fontLabel, XBrushes.Black, 40, y);
                y += 15;
            }

            // ✅ CORRIGIDO - evita null
            string[] linhas = (valor ?? "").Split('\n');
            foreach (var linha in linhas)
            {
                gfx.DrawString(linha.Trim(), fontValor, XBrushes.DarkGray, 50, y);
                y += 14;
            }
            y += 4;
        }
    }
}