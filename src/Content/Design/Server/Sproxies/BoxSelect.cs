namespace Crudspa.Content.Design.Server.Sproxies;

public static class BoxSelect
{
    public static async Task<Box?> Execute(String connection, Guid? sessionId, Box box)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.BoxSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", box.Id);

        return await command.ReadSingle(connection, ReadBox);
    }

    private static Box ReadBox(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            BackgroundColor = reader.ReadString(1),
            BackgroundImageFile = new()
            {
                Id = reader.ReadGuid(2),
                BlobId = reader.ReadGuid(3),
                Name = reader.ReadString(4),
                Format = reader.ReadString(5),
                Width = reader.ReadInt32(6),
                Height = reader.ReadInt32(7),
                Caption = reader.ReadString(8),
            },
            BorderColor = reader.ReadString(9),
            BorderRadius = reader.ReadString(10),
            BorderThickness = reader.ReadString(11),
            BorderThicknessTop = reader.ReadString(12),
            BorderThicknessLeft = reader.ReadString(13),
            BorderThicknessRight = reader.ReadString(14),
            BorderThicknessBottom = reader.ReadString(15),
            CustomFontIndex = reader.ReadEnum<Box.CustomFonts>(16),
            FontSize = reader.ReadString(17),
            FontWeight = reader.ReadString(18),
            ForegroundColor = reader.ReadString(19),
            MarginBottom = reader.ReadString(20),
            MarginLeft = reader.ReadString(21),
            MarginRight = reader.ReadString(22),
            MarginTop = reader.ReadString(23),
            PaddingBottom = reader.ReadString(24),
            PaddingLeft = reader.ReadString(25),
            PaddingRight = reader.ReadString(26),
            PaddingTop = reader.ReadString(27),
            ShadowBlurRadius = reader.ReadString(28),
            ShadowColor = reader.ReadString(29),
            ShadowOffsetX = reader.ReadString(30),
            ShadowOffsetY = reader.ReadString(31),
            ShadowSpreadRadius = reader.ReadString(32),
            TextShadowBlurRadius = reader.ReadString(33),
            TextShadowColor = reader.ReadString(34),
            TextShadowOffsetX = reader.ReadString(35),
            TextShadowOffsetY = reader.ReadString(36),
            HeadingLineHeight = reader.ReadString(37),
            ParagraphLineHeight = reader.ReadString(38),
        };
    }
}