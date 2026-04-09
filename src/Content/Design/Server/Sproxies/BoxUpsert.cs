namespace Crudspa.Content.Design.Server.Sproxies;

public static class BoxUpsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Box box)
    {
        if (box.Id.HasNothing())
            box.Id = Guid.NewGuid();

        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.BoxUpsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", box.Id);
        command.AddParameter("@BackgroundColor", 10, box.BackgroundColor);
        command.AddParameter("@BackgroundImageId", box.BackgroundImageFile?.Id);
        command.AddParameter("@BorderColor", 10, box.BorderColor);
        command.AddParameter("@BorderRadius", 10, box.BorderRadius);
        command.AddParameter("@BorderThickness", 10, box.BorderThickness);
        command.AddParameter("@BorderThicknessTop", 10, box.BorderThicknessTop);
        command.AddParameter("@BorderThicknessLeft", 10, box.BorderThicknessLeft);
        command.AddParameter("@BorderThicknessRight", 10, box.BorderThicknessRight);
        command.AddParameter("@BorderThicknessBottom", 10, box.BorderThicknessBottom);
        command.AddParameter("@CustomFontIndex", box.CustomFontIndex);
        command.AddParameter("@FontSize", 10, box.FontSize);
        command.AddParameter("@FontWeight", 10, box.FontWeight);
        command.AddParameter("@ForegroundColor", 10, box.ForegroundColor);
        command.AddParameter("@MarginBottom", 10, box.MarginBottom);
        command.AddParameter("@MarginLeft", 10, box.MarginLeft);
        command.AddParameter("@MarginRight", 10, box.MarginRight);
        command.AddParameter("@MarginTop", 10, box.MarginTop);
        command.AddParameter("@PaddingBottom", 10, box.PaddingBottom);
        command.AddParameter("@PaddingLeft", 10, box.PaddingLeft);
        command.AddParameter("@PaddingRight", 10, box.PaddingRight);
        command.AddParameter("@PaddingTop", 10, box.PaddingTop);
        command.AddParameter("@ShadowBlurRadius", 10, box.ShadowBlurRadius);
        command.AddParameter("@ShadowColor", 10, box.ShadowColor);
        command.AddParameter("@ShadowOffsetX", 10, box.ShadowOffsetX);
        command.AddParameter("@ShadowOffsetY", 10, box.ShadowOffsetY);
        command.AddParameter("@ShadowSpreadRadius", 10, box.ShadowSpreadRadius);
        command.AddParameter("@TextShadowBlurRadius", 10, box.TextShadowBlurRadius);
        command.AddParameter("@TextShadowColor", 10, box.TextShadowColor);
        command.AddParameter("@TextShadowOffsetX", 10, box.TextShadowOffsetX);
        command.AddParameter("@TextShadowOffsetY", 10, box.TextShadowOffsetY);
        command.AddParameter("@HeadingLineHeight", 10, box.HeadingLineHeight);
        command.AddParameter("@ParagraphLineHeight", 10, box.ParagraphLineHeight);

        await command.Execute(connection, transaction);

        return box.Id;
    }
}