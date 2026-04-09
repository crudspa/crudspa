using System.Text.RegularExpressions;

namespace Crudspa.Framework.Core.Server.Services;

public class CssInlinerPreMailer : ICssInliner
{
    public String InlineCss(String html)
    {
        var replaced = Regex.Replace(html, "<p>\\s*</p>", "<p>&nbsp;</p>", RegexOptions.IgnoreCase);

        var result = PreMailer.Net.PreMailer.MoveCssInline(replaced, removeComments: true);

        return result.Html;
    }
}