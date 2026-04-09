namespace Crudspa.Content.Design.Server.Sproxies;

public static class PageSelect
{
    public static async Task<Page?> Execute(String connection, Guid? sessionId, Page page)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PageSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", page.Id);

        return await command.ReadSingle(connection, ReadPage);
    }

    public static Page ReadPage(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            BinderId = reader.ReadGuid(1),
            Title = reader.ReadString(2),
            TypeId = reader.ReadGuid(3),
            TypeName = reader.ReadString(4),
            StatusId = reader.ReadGuid(5),
            StatusName = reader.ReadString(6),
            GuideText = reader.ReadString(7),
            GuideAudioFile = new()
            {
                Id = reader.ReadGuid(8),
                BlobId = reader.ReadGuid(9),
                Name = reader.ReadString(10),
                Format = reader.ReadString(11),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(12),
                OptimizedBlobId = reader.ReadGuid(13),
                OptimizedFormat = reader.ReadString(14),
            },
            ShowNotebook = reader.ReadBoolean(15),
            ShowGuide = reader.ReadBoolean(16),
            Ordinal = reader.ReadInt32(17),
            Box = new()
            {
                Id = reader.ReadGuid(18),
                BackgroundColor = reader.ReadString(19),
                BackgroundImageFile = new()
                {
                    Id = reader.ReadGuid(20),
                    BlobId = reader.ReadGuid(21),
                    Name = reader.ReadString(22),
                    Format = reader.ReadString(23),
                    Width = reader.ReadInt32(24),
                    Height = reader.ReadInt32(25),
                    Caption = reader.ReadString(26),
                },
                BorderColor = reader.ReadString(27),
                BorderRadius = reader.ReadString(28),
                BorderThickness = reader.ReadString(29),
                BorderThicknessTop = reader.ReadString(30),
                BorderThicknessLeft = reader.ReadString(31),
                BorderThicknessRight = reader.ReadString(32),
                BorderThicknessBottom = reader.ReadString(33),
                CustomFontIndex = reader.ReadEnum<Box.CustomFonts>(34),
                FontSize = reader.ReadString(35),
                FontWeight = reader.ReadString(36),
                ForegroundColor = reader.ReadString(37),
                MarginBottom = reader.ReadString(38),
                MarginLeft = reader.ReadString(39),
                MarginRight = reader.ReadString(40),
                MarginTop = reader.ReadString(41),
                PaddingBottom = reader.ReadString(42),
                PaddingLeft = reader.ReadString(43),
                PaddingRight = reader.ReadString(44),
                PaddingTop = reader.ReadString(45),
                ShadowBlurRadius = reader.ReadString(46),
                ShadowColor = reader.ReadString(47),
                ShadowOffsetX = reader.ReadString(48),
                ShadowOffsetY = reader.ReadString(49),
                ShadowSpreadRadius = reader.ReadString(50),
                TextShadowBlurRadius = reader.ReadString(51),
                TextShadowColor = reader.ReadString(52),
                TextShadowOffsetX = reader.ReadString(53),
                TextShadowOffsetY = reader.ReadString(54),
                HeadingLineHeight = reader.ReadString(55),
                ParagraphLineHeight = reader.ReadString(56),
            },
            SectionCount = reader.ReadInt32(57),
        };
    }
}