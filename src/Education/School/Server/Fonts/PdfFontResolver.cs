using PdfSharp.Fonts;

namespace Crudspa.Education.School.Server.Fonts;

public class PdfFontResolver(IEmbeddedResourceService embeddedResourceService) : IFontResolver
{
    public FontResolverInfo ResolveTypeface(String familyName, Boolean isBold, Boolean isItalic)
    {
        return new("Lato#");
    }

    public Byte[] GetFont(String faceName)
    {
        var assembly = typeof(PdfFontResolver).Assembly;
        return embeddedResourceService.ReadBytesNow(assembly, "Crudspa.Education.School.Server.Fonts.Lato-Regular.ttf");
    }
}