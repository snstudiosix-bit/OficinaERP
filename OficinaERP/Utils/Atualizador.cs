using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OficinaERP.Utils
{
    public static class Atualizador
    {
        private const string UrlVersao =
            "https://raw.githubusercontent.com/snstudiosix-bit/OficinaERP/main/version.json";

        public static async Task VerificarAtualizacao()
        {
            try
            {
                using HttpClient client = new();

                string json = await client.GetStringAsync(UrlVersao);

                using JsonDocument doc = JsonDocument.Parse(json);

                string versaoOnline =
                    doc.RootElement.GetProperty("version").GetString();

                string versaoAtual = Application.ProductVersion;

                if (versaoOnline != versaoAtual)
                {
                    MessageBox.Show(
                        $"Nova versão disponível!\n\nVersão atual: {versaoAtual}\nNova versão: {versaoOnline}",
                        "Atualização",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch
            {
                // Ignora erro caso não tenha internet
            }
        }
    }
}