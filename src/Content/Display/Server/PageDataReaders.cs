namespace Crudspa.Content.Display.Server;

public static class PageDataReaders
{
    public static Page ReadPage(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            BinderId = reader.ReadGuid(1),
            Title = reader.ReadString(2),
            ShowNotebook = reader.ReadBoolean(3),
            ShowGuide = reader.ReadBoolean(4),
            GuideText = reader.ReadString(5),
            GuideAudioFile = new()
            {
                Id = reader.ReadGuid(6),
                Name = reader.ReadString(7),
                Format = reader.ReadString(8),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(9),
                OptimizedBlobId = reader.ReadGuid(10),
                OptimizedFormat = reader.ReadString(11),
            },
            Ordinal = reader.ReadInt32(12),
            Box = new()
            {
                Id = reader.ReadGuid(13),
                BackgroundColor = reader.ReadString(14),
                BackgroundImageFile = new()
                {
                    Id = reader.ReadGuid(15),
                    BlobId = reader.ReadGuid(16),
                    Name = reader.ReadString(17),
                    Format = reader.ReadString(18),
                    Width = reader.ReadInt32(19),
                    Height = reader.ReadInt32(20),
                    Caption = reader.ReadString(21),
                },
                BorderColor = reader.ReadString(22),
                BorderRadius = reader.ReadString(23),
                BorderThickness = reader.ReadString(24),
                BorderThicknessTop = reader.ReadString(25),
                BorderThicknessLeft = reader.ReadString(26),
                BorderThicknessRight = reader.ReadString(27),
                BorderThicknessBottom = reader.ReadString(28),
                CustomFontIndex = reader.ReadEnum<Box.CustomFonts>(29),
                FontSize = reader.ReadString(30),
                FontWeight = reader.ReadString(31),
                ForegroundColor = reader.ReadString(32),
                MarginBottom = reader.ReadString(33),
                MarginLeft = reader.ReadString(34),
                MarginRight = reader.ReadString(35),
                MarginTop = reader.ReadString(36),
                PaddingBottom = reader.ReadString(37),
                PaddingLeft = reader.ReadString(38),
                PaddingRight = reader.ReadString(39),
                PaddingTop = reader.ReadString(40),
                ShadowBlurRadius = reader.ReadString(41),
                ShadowColor = reader.ReadString(42),
                ShadowOffsetX = reader.ReadString(43),
                ShadowOffsetY = reader.ReadString(44),
                ShadowSpreadRadius = reader.ReadString(45),
                TextShadowBlurRadius = reader.ReadString(46),
                TextShadowColor = reader.ReadString(47),
                TextShadowOffsetX = reader.ReadString(48),
                TextShadowOffsetY = reader.ReadString(49),
                HeadingLineHeight = reader.ReadString(50),
                ParagraphLineHeight = reader.ReadString(51),
            },
        };
    }

    public static Section ReadSection(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PageId = reader.ReadGuid(1),
            Container = new()
            {
                Id = reader.ReadGuid(2),
                DirectionId = reader.ReadGuid(3),
                WrapId = reader.ReadGuid(4),
                JustifyContentId = reader.ReadGuid(5),
                AlignItemsId = reader.ReadGuid(6),
                AlignContentId = reader.ReadGuid(7),
                Gap = reader.ReadString(8),
            },
            Ordinal = reader.ReadInt32(9),
            TypeId = reader.ReadGuid(10),
            Box = new()
            {
                Id = reader.ReadGuid(11),
                BackgroundColor = reader.ReadString(12),
                BackgroundImageFile = new()
                {
                    Id = reader.ReadGuid(13),
                    BlobId = reader.ReadGuid(14),
                    Name = reader.ReadString(15),
                    Format = reader.ReadString(16),
                    Width = reader.ReadInt32(17),
                    Height = reader.ReadInt32(18),
                    Caption = reader.ReadString(19),
                },
                BorderColor = reader.ReadString(20),
                BorderRadius = reader.ReadString(21),
                BorderThickness = reader.ReadString(22),
                BorderThicknessTop = reader.ReadString(23),
                BorderThicknessLeft = reader.ReadString(24),
                BorderThicknessRight = reader.ReadString(25),
                BorderThicknessBottom = reader.ReadString(26),
                CustomFontIndex = reader.ReadEnum<Box.CustomFonts>(27),
                FontSize = reader.ReadString(28),
                FontWeight = reader.ReadString(29),
                ForegroundColor = reader.ReadString(30),
                MarginBottom = reader.ReadString(31),
                MarginLeft = reader.ReadString(32),
                MarginRight = reader.ReadString(33),
                MarginTop = reader.ReadString(34),
                PaddingBottom = reader.ReadString(35),
                PaddingLeft = reader.ReadString(36),
                PaddingRight = reader.ReadString(37),
                PaddingTop = reader.ReadString(38),
                ShadowBlurRadius = reader.ReadString(39),
                ShadowColor = reader.ReadString(40),
                ShadowOffsetX = reader.ReadString(41),
                ShadowOffsetY = reader.ReadString(42),
                ShadowSpreadRadius = reader.ReadString(43),
                TextShadowBlurRadius = reader.ReadString(44),
                TextShadowColor = reader.ReadString(45),
                TextShadowOffsetX = reader.ReadString(46),
                TextShadowOffsetY = reader.ReadString(47),
                HeadingLineHeight = reader.ReadString(48),
                ParagraphLineHeight = reader.ReadString(49),
            },
        };
    }

    public static Element ReadElement(SqlDataReader reader)
    {
        var element = new Element
        {
            Id = reader.ReadGuid(0),
            SectionId = reader.ReadGuid(1),
            TypeId = reader.ReadGuid(2),
            Item = new()
            {
                Id = reader.ReadGuid(3),
                BasisId = reader.ReadGuid(4),
                BasisAmount = reader.ReadString(5),
                Grow = reader.ReadString(6),
                Shrink = reader.ReadString(7),
                AlignSelfId = reader.ReadGuid(8),
                MaxWidth = reader.ReadString(9),
                MinWidth = reader.ReadString(10),
                Width = reader.ReadString(11),
            },
            RequireInteraction = reader.ReadBoolean(12),
            Ordinal = reader.ReadInt32(13),
            ElementType = new()
            {
                EditorView = reader.ReadString(14),
                DisplayView = reader.ReadString(15),
                RepositoryClass = reader.ReadString(16),
                OnlyChild = reader.ReadBoolean(17),
                SupportsInteraction = reader.ReadBoolean(18),
                IconId = reader.ReadGuid(19),
                IconCssClass = reader.ReadString(20),
            },
            Box = new()
            {
                Id = reader.ReadGuid(21),
                BackgroundColor = reader.ReadString(22),
                BackgroundImageFile = new()
                {
                    Id = reader.ReadGuid(23),
                    BlobId = reader.ReadGuid(24),
                    Name = reader.ReadString(25),
                    Format = reader.ReadString(26),
                    Width = reader.ReadInt32(27),
                    Height = reader.ReadInt32(28),
                    Caption = reader.ReadString(29),
                },
                BorderColor = reader.ReadString(30),
                BorderRadius = reader.ReadString(31),
                BorderThickness = reader.ReadString(32),
                BorderThicknessTop = reader.ReadString(33),
                BorderThicknessLeft = reader.ReadString(34),
                BorderThicknessRight = reader.ReadString(35),
                BorderThicknessBottom = reader.ReadString(36),
                CustomFontIndex = reader.ReadEnum<Box.CustomFonts>(37),
                FontSize = reader.ReadString(38),
                FontWeight = reader.ReadString(39),
                ForegroundColor = reader.ReadString(40),
                MarginBottom = reader.ReadString(41),
                MarginLeft = reader.ReadString(42),
                MarginRight = reader.ReadString(43),
                MarginTop = reader.ReadString(44),
                PaddingBottom = reader.ReadString(45),
                PaddingLeft = reader.ReadString(46),
                PaddingRight = reader.ReadString(47),
                PaddingTop = reader.ReadString(48),
                ShadowBlurRadius = reader.ReadString(49),
                ShadowColor = reader.ReadString(50),
                ShadowOffsetX = reader.ReadString(51),
                ShadowOffsetY = reader.ReadString(52),
                ShadowSpreadRadius = reader.ReadString(53),
                TextShadowBlurRadius = reader.ReadString(54),
                TextShadowColor = reader.ReadString(55),
                TextShadowOffsetX = reader.ReadString(56),
                TextShadowOffsetY = reader.ReadString(57),
                HeadingLineHeight = reader.ReadString(58),
                ParagraphLineHeight = reader.ReadString(59),
            },
        };

        return element;
    }

    public static AudioElement ReadAudio(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ElementId = reader.ReadGuid(1),
            FileFile = new()
            {
                Id = reader.ReadGuid(2),
                BlobId = reader.ReadGuid(3),
                Name = reader.ReadString(4),
                Format = reader.ReadString(5),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(6),
                OptimizedBlobId = reader.ReadGuid(7),
                OptimizedFormat = reader.ReadString(8),
            },
        };
    }

    public static TextElement ReadTextElement(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ElementId = reader.ReadGuid(1),
            Text = reader.ReadString(2),
        };
    }

    public static ImageElement ReadImage(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ElementId = reader.ReadGuid(1),
            FileFile = new()
            {
                Id = reader.ReadGuid(2),
                BlobId = reader.ReadGuid(3),
                Name = reader.ReadString(4),
                Format = reader.ReadString(5),
                Width = reader.ReadInt32(6),
                Height = reader.ReadInt32(7),
                Caption = reader.ReadString(8),
            },
            HyperlinkUrl = reader.ReadString(9),
        };
    }

    public static ButtonElement ReadButtonElement(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ElementId = reader.ReadGuid(1),
            Button = ReadButton(reader, 2, 19),
        };
    }

    public static MultimediaElement ReadMultimediaElement(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ElementId = reader.ReadGuid(1),
            Container = new()
            {
                Id = reader.ReadGuid(2),
                DirectionId = reader.ReadGuid(3),
                WrapId = reader.ReadGuid(4),
                JustifyContentId = reader.ReadGuid(5),
                AlignItemsId = reader.ReadGuid(6),
                AlignContentId = reader.ReadGuid(7),
                Gap = reader.ReadString(8),
            },
        };
    }

    public static MultimediaItem ReadMultimediaItem(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            MultimediaElementId = reader.ReadGuid(1),
            Box = new()
            {
                Id = reader.ReadGuid(2),
                BackgroundColor = reader.ReadString(3),
                BackgroundImageFile = new()
                {
                    Id = reader.ReadGuid(4),
                    BlobId = reader.ReadGuid(5),
                    Name = reader.ReadString(6),
                    Format = reader.ReadString(7),
                    Width = reader.ReadInt32(8),
                    Height = reader.ReadInt32(9),
                    Caption = reader.ReadString(10),
                },
                BorderColor = reader.ReadString(11),
                BorderRadius = reader.ReadString(12),
                BorderThickness = reader.ReadString(13),
                BorderThicknessTop = reader.ReadString(14),
                BorderThicknessLeft = reader.ReadString(15),
                BorderThicknessRight = reader.ReadString(16),
                BorderThicknessBottom = reader.ReadString(17),
                CustomFontIndex = reader.ReadEnum<Box.CustomFonts>(18),
                FontSize = reader.ReadString(19),
                FontWeight = reader.ReadString(20),
                ForegroundColor = reader.ReadString(21),
                MarginBottom = reader.ReadString(22),
                MarginLeft = reader.ReadString(23),
                MarginRight = reader.ReadString(24),
                MarginTop = reader.ReadString(25),
                PaddingBottom = reader.ReadString(26),
                PaddingLeft = reader.ReadString(27),
                PaddingRight = reader.ReadString(28),
                PaddingTop = reader.ReadString(29),
                ShadowBlurRadius = reader.ReadString(30),
                ShadowColor = reader.ReadString(31),
                ShadowOffsetX = reader.ReadString(32),
                ShadowOffsetY = reader.ReadString(33),
                ShadowSpreadRadius = reader.ReadString(34),
                TextShadowBlurRadius = reader.ReadString(35),
                TextShadowColor = reader.ReadString(36),
                TextShadowOffsetX = reader.ReadString(37),
                TextShadowOffsetY = reader.ReadString(38),
                HeadingLineHeight = reader.ReadString(39),
                ParagraphLineHeight = reader.ReadString(40),
            },
            Item = new()
            {
                Id = reader.ReadGuid(41),
                BasisId = reader.ReadGuid(42),
                BasisAmount = reader.ReadString(43),
                Grow = reader.ReadString(44),
                Shrink = reader.ReadString(45),
                AlignSelfId = reader.ReadGuid(46),
                MaxWidth = reader.ReadString(47),
                MinWidth = reader.ReadString(48),
                Width = reader.ReadString(49),
            },
            MediaTypeIndex = reader.ReadEnum<MultimediaItem.MediaTypes>(50),
            AudioFile = new()
            {
                Id = reader.ReadGuid(51),
                BlobId = reader.ReadGuid(52),
                Name = reader.ReadString(53),
                Format = reader.ReadString(54),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(55),
                OptimizedBlobId = reader.ReadGuid(56),
                OptimizedFormat = reader.ReadString(57),
            },
            Button = ReadButton(reader, 58, 75),
            ImageFile = new()
            {
                Id = reader.ReadGuid(114),
                BlobId = reader.ReadGuid(115),
                Name = reader.ReadString(116),
                Format = reader.ReadString(117),
                Width = reader.ReadInt32(118),
                Height = reader.ReadInt32(119),
                Caption = reader.ReadString(120),
            },
            Text = reader.ReadString(121),
            VideoFile = new()
            {
                Id = reader.ReadGuid(122),
                BlobId = reader.ReadGuid(123),
                Name = reader.ReadString(124),
                Format = reader.ReadString(125),
                Width = reader.ReadInt32(126),
                Height = reader.ReadInt32(127),
                OptimizedStatus = reader.ReadEnum<VideoFile.OptimizationStatus>(128),
                OptimizedBlobId = reader.ReadGuid(129),
                OptimizedFormat = reader.ReadString(130),
            },
            Ordinal = reader.ReadInt32(131),
        };
    }

    private static Button ReadButton(SqlDataReader reader, Int32 buttonStart, Int32 boxStart)
    {
        return new()
        {
            Id = reader.ReadGuid(buttonStart),
            Internal = reader.ReadBoolean(buttonStart + 1),
            Path = reader.ReadString(buttonStart + 2),
            Text = reader.ReadString(buttonStart + 3),
            ShapeIndex = reader.ReadEnum<Button.Shapes>(buttonStart + 4),
            GraphicIndex = reader.ReadEnum<Button.Graphics>(buttonStart + 5),
            TextTypeIndex = reader.ReadEnum<Button.TextTypes>(buttonStart + 6),
            OrientationIndex = reader.ReadEnum<Button.Orientations>(buttonStart + 7),
            IconId = reader.ReadGuid(buttonStart + 8),
            ImageFile = new()
            {
                Id = reader.ReadGuid(buttonStart + 9),
                BlobId = reader.ReadGuid(buttonStart + 10),
                Name = reader.ReadString(buttonStart + 11),
                Format = reader.ReadString(buttonStart + 12),
                Width = reader.ReadInt32(buttonStart + 13),
                Height = reader.ReadInt32(buttonStart + 14),
                Caption = reader.ReadString(buttonStart + 15),
            },
            IconCssClass = reader.ReadString(buttonStart + 16),
            Box = new()
            {
                Id = reader.ReadGuid(boxStart),
                BackgroundColor = reader.ReadString(boxStart + 1),
                BackgroundImageFile = new()
                {
                    Id = reader.ReadGuid(boxStart + 2),
                    BlobId = reader.ReadGuid(boxStart + 3),
                    Name = reader.ReadString(boxStart + 4),
                    Format = reader.ReadString(boxStart + 5),
                    Width = reader.ReadInt32(boxStart + 6),
                    Height = reader.ReadInt32(boxStart + 7),
                    Caption = reader.ReadString(boxStart + 8),
                },
                BorderColor = reader.ReadString(boxStart + 9),
                BorderRadius = reader.ReadString(boxStart + 10),
                BorderThickness = reader.ReadString(boxStart + 11),
                BorderThicknessTop = reader.ReadString(boxStart + 12),
                BorderThicknessLeft = reader.ReadString(boxStart + 13),
                BorderThicknessRight = reader.ReadString(boxStart + 14),
                BorderThicknessBottom = reader.ReadString(boxStart + 15),
                CustomFontIndex = reader.ReadEnum<Box.CustomFonts>(boxStart + 16),
                FontSize = reader.ReadString(boxStart + 17),
                FontWeight = reader.ReadString(boxStart + 18),
                ForegroundColor = reader.ReadString(boxStart + 19),
                MarginBottom = reader.ReadString(boxStart + 20),
                MarginLeft = reader.ReadString(boxStart + 21),
                MarginRight = reader.ReadString(boxStart + 22),
                MarginTop = reader.ReadString(boxStart + 23),
                PaddingBottom = reader.ReadString(boxStart + 24),
                PaddingLeft = reader.ReadString(boxStart + 25),
                PaddingRight = reader.ReadString(boxStart + 26),
                PaddingTop = reader.ReadString(boxStart + 27),
                ShadowBlurRadius = reader.ReadString(boxStart + 28),
                ShadowColor = reader.ReadString(boxStart + 29),
                ShadowOffsetX = reader.ReadString(boxStart + 30),
                ShadowOffsetY = reader.ReadString(boxStart + 31),
                ShadowSpreadRadius = reader.ReadString(boxStart + 32),
                TextShadowBlurRadius = reader.ReadString(boxStart + 33),
                TextShadowColor = reader.ReadString(boxStart + 34),
                TextShadowOffsetX = reader.ReadString(boxStart + 35),
                TextShadowOffsetY = reader.ReadString(boxStart + 36),
                HeadingLineHeight = reader.ReadString(boxStart + 37),
                ParagraphLineHeight = reader.ReadString(boxStart + 38),
            },
        };
    }

    public static NoteElement ReadNote(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ElementId = reader.ReadGuid(1),
            Instructions = reader.ReadString(2),
            ImageFileFile = new()
            {
                Id = reader.ReadGuid(3),
                BlobId = reader.ReadGuid(4),
                Name = reader.ReadString(5),
                Format = reader.ReadString(6),
                Width = reader.ReadInt32(7),
                Height = reader.ReadInt32(8),
                Caption = reader.ReadString(9),
            },
            RequireText = reader.ReadBoolean(10),
            RequireImageSelection = reader.ReadBoolean(11),
        };
    }

    public static NoteImage ReadNoteImage(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            NoteId = reader.ReadGuid(1),
            ImageFileId = reader.ReadGuid(2),
            ImageFile = new()
            {
                Id = reader.ReadGuid(3),
                BlobId = reader.ReadGuid(4),
                Name = reader.ReadString(5),
                Format = reader.ReadString(6),
                Width = reader.ReadInt32(7),
                Height = reader.ReadInt32(8),
                Caption = reader.ReadString(9),
            },
            Ordinal = reader.ReadInt32(10),
        };
    }

    public static PdfElement ReadPdf(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ElementId = reader.ReadGuid(1),
            FileFile = new()
            {
                Id = reader.ReadGuid(2),
                BlobId = reader.ReadGuid(3),
                Name = reader.ReadString(4),
                Format = reader.ReadString(5),
                Description = reader.ReadString(6),
            },
        };
    }

    public static VideoElement ReadVideo(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ElementId = reader.ReadGuid(1),
            FileFile = new()
            {
                Id = reader.ReadGuid(2),
                BlobId = reader.ReadGuid(3),
                Name = reader.ReadString(4),
                Format = reader.ReadString(5),
                Width = reader.ReadInt32(6),
                Height = reader.ReadInt32(7),
                OptimizedStatus = reader.ReadEnum<VideoFile.OptimizationStatus>(8),
                OptimizedBlobId = reader.ReadGuid(9),
                OptimizedFormat = reader.ReadString(10),
            },
            AutoPlay = reader.ReadBoolean(11),
            Poster = new()
            {
                Id = reader.ReadGuid(12),
                BlobId = reader.ReadGuid(13),
                Name = reader.ReadString(14),
                Format = reader.ReadString(15),
                Width = reader.ReadInt32(16),
                Height = reader.ReadInt32(17),
                Caption = reader.ReadString(18),
            },
        };
    }
}