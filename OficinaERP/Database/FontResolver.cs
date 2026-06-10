// FontResolver.cs
using PdfSharp.Fonts;
using System.IO;
using System.Windows.Forms;

namespace OficinaERP
{
    public class OFontResolver : IFontResolver
    {
        public string DefaultFontName => "OpenSans";

        public byte[] GetFont(string faceName)
        {
            // Usa a fonte do sistema Windows diretamente
            string winFonts = System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.Fonts);

            string arquivo = faceName switch
            {
                "OpenSans#Bold" => Path.Combine(winFonts, "arialbd.ttf"),
                "OpenSans#Italic" => Path.Combine(winFonts, "ariali.ttf"),
                _ => Path.Combine(winFonts, "arial.ttf")
            };

            if (!File.Exists(arquivo))
                arquivo = Path.Combine(winFonts, "segoeui.ttf");

            return File.ReadAllBytes(arquivo);
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (isBold) return new FontResolverInfo("OpenSans#Bold");
            if (isItalic) return new FontResolverInfo("OpenSans#Italic");
            return new FontResolverInfo("OpenSans");
        }
    }
}