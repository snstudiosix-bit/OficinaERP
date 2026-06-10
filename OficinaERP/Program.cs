using OficinaERP.Database;
using OficinaERP.Forms;
using PdfSharp.Fonts;

namespace OficinaERP
{
    internal static class Program
    {
        // ✅ Usuário logado disponível globalmente
        public static string UsuarioNome { get; private set; } = "";
        public static string UsuarioPerfil { get; private set; } = "";

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            GlobalFontSettings.FontResolver = new OFontResolver();
            Banco.CriarBanco();

            // ✅ Mostrar login antes do sistema
            var login = new FrmLogin();
            if (login.ShowDialog() == DialogResult.OK && login.LoginOk)
            {
                UsuarioNome = login.NomeUsuario;
                UsuarioPerfil = login.PerfilUsuario;

                // Verifica atualização
                OficinaERP.Utils.Atualizador.VerificarAtualizacao().Wait();

                Application.Run(new FrmPrincipal());
            }
        }
    }
}