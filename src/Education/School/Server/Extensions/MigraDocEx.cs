using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using Font = MigraDoc.DocumentObjectModel.Font;
using Section = MigraDoc.DocumentObjectModel.Section;

namespace Crudspa.Education.School.Server.Extensions;

public static class MigraDocEx
{
    extension(Document document)
    {
        public Byte[] RenderToBytes()
        {
            var renderer = new PdfDocumentRenderer { Document = document };

            renderer.RenderDocument();

            using var stream = new MemoryStream();
            renderer.PdfDocument.Save(stream, true);
            return stream.ToArray();
        }
    }

    extension(Section section)
    {
        public Double Height()
        {
            return section.PageSetup.PageHeight.Point
                - section.PageSetup.TopMargin.Point
                - section.PageSetup.BottomMargin.Point;
        }

        public Double Width()
        {
            return section.PageSetup.PageWidth.Point
                - section.PageSetup.LeftMargin.Point
                - section.PageSetup.RightMargin.Point;
        }

        public Paragraph AddHorizontalRule(Single width = 0, Color? color = null)
        {
            var paragraph = section.AddParagraph();
            paragraph.AddHorizontalRule(width, color);
            return paragraph;
        }

        public void AddSpacer(Single height)
        {
            var paragraph = section.AddParagraph();
            paragraph.Format.LineSpacing = height;
            paragraph.Format.LineSpacingRule = LineSpacingRule.Exactly;
        }

        public void SetBackgroundColor(Color color)
        {
            var backgroundTextFrame = section.Headers.Primary.AddTextFrame();
            backgroundTextFrame.FillFormat.Color = color;
            backgroundTextFrame.RelativeHorizontal = RelativeHorizontal.Page;
            backgroundTextFrame.RelativeVertical = RelativeVertical.Page;
            backgroundTextFrame.Width = section.PageSetup.PageWidth;
            backgroundTextFrame.Height = section.PageSetup.PageHeight;
        }
    }

    extension(Section section)
    {
        public TextFrame TextFrame()
        {
            var textFrame = section.AddTextFrame();
            textFrame.RelativeVertical = RelativeVertical.Margin;
            textFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            return textFrame;
        }

        public TextFrame TextFrame(Single top, Single left, Single height, Single width)
        {
            var textFrame = section.TextFrame();
            textFrame.Top = top;
            textFrame.Left = left;
            textFrame.Height = height;
            textFrame.Width = width;
            return textFrame;
        }

        public Table Table(params Single[] widths)
        {
            var table = section.AddTable();
            table.AddColumns(widths);
            return table;
        }
    }

    extension(TextFrame textFrame)
    {
        public TextFrame FillColor(Color fillColor)
        {
            textFrame.FillFormat.Color = fillColor;
            return textFrame;
        }

        public TextFrame AddSpacer(Single height)
        {
            var paragraph = textFrame.AddParagraph();
            paragraph.Format.LineSpacing = height;
            paragraph.Format.LineSpacingRule = LineSpacingRule.Exactly;
            return textFrame;
        }

        public Table Table(params Single[] widths)
        {
            var table = textFrame.AddTable();
            table.AddColumns(widths);
            return table;
        }
    }

    extension(Table table)
    {
        public Table AddColumns(params Single[] widths)
        {
            // A value of 0 is treated as "*" -- remaining space is distributed evenly
            var numberOfStars = widths.Count(x => x == 0);
            var spacePerStar = (table.Section!.Width() - widths.Sum()) / numberOfStars;

            foreach (var width in widths)
                table.AddColumn(width == 0 ? spacePerStar : width);

            return table;
        }

        public Table AddRows(Int32 count, Single height = 0, VerticalAlignment verticalAlignment = VerticalAlignment.Top)
        {
            for (var i = 0; i < count; i++)
            {
                var row = table.AddRow();
                if (height > 0)
                {
                    row.Height = height;
                    row.HeightRule = RowHeightRule.AtLeast;
                }

                row.VerticalAlignment = verticalAlignment;
            }

            return table;
        }

        public Table Borders(Single borderWidth = 0)
        {
            table.Borders.Visible = true;
            if (borderWidth != 0) table.Borders.Width = borderWidth;
            return table;
        }

        public Table RemoveBottomBorder()
        {
            table.Rows[table.Rows.Count - 1].Borders.Bottom.Visible = false;
            return table;
        }

        public Table AddOuterBorders(Single borderWidth = 0)
        {
            table.TopBorder(borderWidth);
            table.BottomBorder(borderWidth);
            table.SideBorders(borderWidth);
            return table;
        }

        public Table TopAndSideBorders(Single borderWidth = 0)
        {
            table.TopBorder(borderWidth);
            table.SideBorders(borderWidth);
            return table;
        }

        public Table BottomAndSideBorders(Single borderWidth = 0)
        {
            table.BottomBorder(borderWidth);
            table.SideBorders(borderWidth);
            return table;
        }

        public Table TopBorder(Single borderWidth = 0)
        {
            table.Rows[0].Borders.Top.Visible = true;
            if (borderWidth != 0) table.Rows[0].Borders.Top.Width = borderWidth;
            return table;
        }

        public Table BottomBorder(Single borderWidth = 0)
        {
            table.Rows[table.Rows.Count - 1].Borders.Bottom.Visible = true;
            if (borderWidth != 0) table.Rows[table.Rows.Count - 1].Borders.Bottom.Width = borderWidth;
            return table;
        }

        public Table SideBorders(Single borderWidth = 0)
        {
            table.Columns[0].Borders.Left.Visible = true;
            table.Columns[table.Columns.Count - 1].Borders.Right.Visible = true;
            if (borderWidth != 0)
            {
                table.Columns[0].Borders.Left.Width = borderWidth;
                table.Columns[table.Columns.Count - 1].Borders.Right.Width = borderWidth;
            }

            return table;
        }

        public Table Center()
        {
            table.Format.Alignment = ParagraphAlignment.Center;
            return table;
        }

        public Table Font(String fontName)
        {
            table.Format.Font.Name = fontName;
            return table;
        }

        public Table Size(Single size)
        {
            table.Format.Font.Size = size;
            return table;
        }

        public Table Bold(Boolean bold = true)
        {
            table.Format.Font.Bold = bold;
            return table;
        }

        public Table Italic(Boolean italic = true)
        {
            table.Format.Font.Italic = italic;
            return table;
        }

        public Table UnderlineIt(Boolean underline = true)
        {
            table.Format.Font.Underline = underline ? Underline.Single : Underline.None;
            return table;
        }

        public Table BackgroundColor(Color color)
        {
            table.Shading.Color = color;
            return table;
        }

        public Table Color(Color color)
        {
            table.Format.Font.Color = color;
            return table;
        }

        public Table LineSpacing(Single lineSpacing)
        {
            table.Format.LineSpacing = lineSpacing;
            table.Format.LineSpacingRule = LineSpacingRule.Exactly;
            return table;
        }

        public Table ColumnAlignment(ParagraphAlignment alignment, params Int32[] columnIndexes)
        {
            foreach (var i in columnIndexes)
                table.Columns[i].Format.Alignment = alignment;
            return table;
        }

        public Table Shading(Color color)
        {
            table.Shading.Color = color;
            return table;
        }
    }

    extension(Column column)
    {
        public Column Bold(Boolean bold = true)
        {
            column.Format.Font.Bold = bold;
            return column;
        }

        public Column Center()
        {
            column.Format.Alignment = ParagraphAlignment.Center;
            return column;
        }

        public Column Right()
        {
            column.Format.Alignment = ParagraphAlignment.Right;
            return column;
        }

        public Column BottomBorders()
        {
            if (column.Table?.Rows is not null)
                foreach (var row in column.Table!.Rows.Cast<Row>())
                    row![column.Index].Borders.Bottom.Visible = true;
            return column;
        }
    }

    extension(Row row)
    {
        public Row Bold(Boolean bold = true)
        {
            row.Format.Font.Bold = bold;
            return row;
        }

        public Row Italic(Boolean italic = true)
        {
            row.Format.Font.Italic = italic;
            return row;
        }

        public Row Bottom()
        {
            row.VerticalAlignment = VerticalAlignment.Bottom;
            return row;
        }

        public Row BottomBorder(Boolean visible = true)
        {
            row.Borders.Bottom.Visible = visible;
            return row;
        }

        public Row BottomBorders(params Int32[] cellIndexes)
        {
            foreach (var i in cellIndexes)
                row.Cells[i].Borders.Bottom.Visible = true;
            return row;
        }

        public Row TopBorder(Boolean visible = true, Single width = 0)
        {
            row.Borders.Top.Visible = visible;
            if (width != 0) row.Borders.Top.Width = width;
            return row;
        }

        public Row Center()
        {
            row.Format.Alignment = ParagraphAlignment.Center;
            return row;
        }

        public Row Height(Single height, RowHeightRule heightRule = RowHeightRule.AtLeast)
        {
            row.Height = height;
            row.HeightRule = heightRule;
            return row;
        }

        public Row Middle()
        {
            row.VerticalAlignment = VerticalAlignment.Center;
            return row;
        }

        public Row Font(String fontName)
        {
            row.Format.Font.Name = fontName;
            return row;
        }

        public Row Size(Single size)
        {
            row.Format.Font.Size = size;
            return row;
        }
    }

    extension(Cell cell)
    {
        public Cell AllBorders(Single borderWidth = 0)
        {
            cell.Borders.Top.Visible = true;
            cell.Borders.Right.Visible = true;
            cell.Borders.Bottom.Visible = true;
            cell.Borders.Left.Visible = true;

            if (borderWidth != 0)
            {
                cell.Borders.Top.Width = borderWidth;
                cell.Borders.Right.Width = borderWidth;
                cell.Borders.Bottom.Width = borderWidth;
                cell.Borders.Left.Width = borderWidth;
            }

            if (cell.Column!.Index > 0)
            {
                cell.Row![cell.Column.Index - 1].Borders.Right.Visible = true;
                if (borderWidth != 0) cell.Row[cell.Column.Index - 1].Borders.Right.Width = borderWidth;
            }

            if (cell.Row!.Index > 0)
            {
                cell.Table!.Rows[cell.Row.Index - 1][cell.Column.Index].Borders.Bottom.Visible = true;
                if (borderWidth != 0) cell.Table.Rows[cell.Row.Index - 1][cell.Column.Index].Borders.Bottom.Width = borderWidth;
            }

            return cell;
        }

        public Cell Top()
        {
            cell.VerticalAlignment = VerticalAlignment.Top;
            return cell;
        }

        public Cell Middle()
        {
            cell.VerticalAlignment = VerticalAlignment.Center;
            return cell;
        }

        public Cell Bottom()
        {
            cell.VerticalAlignment = VerticalAlignment.Bottom;
            return cell;
        }

        public Cell Left()
        {
            cell.Format.Alignment = ParagraphAlignment.Left;
            return cell;
        }

        public Cell Center()
        {
            cell.Format.Alignment = ParagraphAlignment.Center;
            return cell;
        }

        public Cell Right()
        {
            cell.Format.Alignment = ParagraphAlignment.Right;
            return cell;
        }

        public Cell Justify()
        {
            cell.Format.Alignment = ParagraphAlignment.Justify;
            return cell;
        }

        public Cell Bold(Boolean bold = true)
        {
            cell.Format.Font.Bold = bold;
            return cell;
        }

        public Cell LeftBorder(Boolean visible = true)
        {
            cell.Borders.Left.Visible = visible;

            // Fix for when Left border won't show up
            if (cell.Column!.Index > 0)
                cell.Table![cell.Row!.Index, cell.Column.Index - 1].Borders.Right.Visible = visible;

            return cell;
        }

        public Cell TopBorder(Boolean visible = true)
        {
            cell.Borders.Top.Visible = visible;

            // Fix for when Top border won't show up
            if (cell.Row!.Index > 0)
                cell.Table![cell.Row.Index - 1, cell.Column!.Index].Borders.Bottom.Visible = true;

            return cell;
        }

        public Cell RightBorder(Boolean visible = true)
        {
            cell.Borders.Right.Visible = visible;
            return cell;
        }

        public Cell TopAndSideBorders()
        {
            cell.Borders.Top.Visible = true;
            cell.Borders.Right.Visible = true;
            cell.Borders.Left.Visible = true;

            // Fix for when Top border won't show up
            if (cell.Row!.Index > 0)
                cell.Table![cell.Row.Index - 1, cell.Column!.Index].Borders.Bottom.Visible = true;

            // Fix for when Left border won't show up
            if (cell.Column!.Index > 0)
                cell.Table![cell.Row.Index, cell.Column.Index - 1].Borders.Right.Visible = true;

            return cell;
        }

        public Cell BottomAndSideBorders()
        {
            cell.Borders.Bottom.Visible = true;
            cell.Borders.Right.Visible = true;
            cell.Borders.Left.Visible = true;

            // Fix for when Left border won't show up
            if (cell.Column!.Index > 0)
                cell.Table![cell.Row!.Index, cell.Column.Index - 1].Borders.Right.Visible = true;

            return cell;
        }

        public Cell SideBorders()
        {
            cell.Borders.Right.Visible = true;
            cell.Borders.Left.Visible = true;

            // Fix for when Left border won't show up
            if (cell.Column!.Index > 0)
                cell.Table![cell.Row!.Index, cell.Column.Index - 1].Borders.Right.Visible = true;

            return cell;
        }

        public Cell BottomBorder(Boolean visible = true)
        {
            cell.Borders.Bottom.Visible = visible;
            return cell;
        }

        public Cell Font(String fontName)
        {
            cell.Format.Font.Name = fontName;
            return cell;
        }

        public Cell LineSpacing(Single lineSpacing)
        {
            cell.Format.LineSpacing = lineSpacing;
            cell.Format.LineSpacingRule = LineSpacingRule.Exactly;
            return cell;
        }

        public Cell MergeDown(Int32 count)
        {
            cell.MergeDown = count;
            return cell;
        }

        public Cell MergeRight(Int32 count)
        {
            cell.MergeRight = count;
            return cell;
        }

        public Cell NoBorders()
        {
            cell.Borders.Top.Visible = false;
            cell.Borders.Right.Visible = false;
            cell.Borders.Bottom.Visible = false;
            cell.Borders.Left.Visible = false;
            return cell;
        }

        public Cell Shading(Color color)
        {
            cell.Shading.Color = color;
            return cell;
        }

        public Cell Size(Single size)
        {
            cell.Format.Font.Size = size;
            return cell;
        }

        public Paragraph Paragraph(String text, Font font)
        {
            var paragraph = cell.AddParagraph();
            paragraph.AddFormattedText(text, font);
            return paragraph;
        }
    }

    extension(Paragraph paragraph)
    {
        public Paragraph BackgroundColor(Color color)
        {
            paragraph.Format.Shading.Color = color;
            return paragraph;
        }

        public Paragraph Left()
        {
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            return paragraph;
        }

        public Paragraph Center()
        {
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            return paragraph;
        }

        public Paragraph Right()
        {
            paragraph.Format.Alignment = ParagraphAlignment.Right;
            return paragraph;
        }

        public Paragraph Justify()
        {
            paragraph.Format.Alignment = ParagraphAlignment.Justify;
            return paragraph;
        }

        public Paragraph Font(String fontName)
        {
            paragraph.Format.Font.Name = fontName;
            return paragraph;
        }

        public Paragraph Size(Single size)
        {
            paragraph.Format.Font.Size = size;
            return paragraph;
        }

        public Paragraph Bold(Boolean bold = true)
        {
            paragraph.Format.Font.Bold = bold;
            return paragraph;
        }

        public Paragraph Italic(Boolean italic = true)
        {
            paragraph.Format.Font.Italic = italic;
            return paragraph;
        }

        public Paragraph UnderlineIt(Boolean underline = true)
        {
            paragraph.Format.Font.Underline = underline ? Underline.Single : Underline.None;
            return paragraph;
        }

        public Paragraph Color(Color color)
        {
            paragraph.Format.Font.Color = color;
            return paragraph;
        }

        public Paragraph LeftIndent(Single indent)
        {
            paragraph.Format.LeftIndent = indent;
            return paragraph;
        }

        public Paragraph LineSpacing(Single lineSpacing)
        {
            paragraph.Format.LineSpacing = lineSpacing;
            paragraph.Format.LineSpacingRule = LineSpacingRule.Exactly;
            return paragraph;
        }

        public Paragraph RightIndent(Single indent)
        {
            paragraph.Format.RightIndent = indent;
            return paragraph;
        }

        public Paragraph SpaceAfter(Single space)
        {
            paragraph.Format.SpaceAfter = space;
            return paragraph;
        }

        public Paragraph SpaceBefore(Single space)
        {
            paragraph.Format.SpaceBefore = space;
            return paragraph;
        }

        public Paragraph Text(String text)
        {
            paragraph.AddText(text);
            return paragraph;
        }

        public Paragraph Text(String text, Font font)
        {
            paragraph.AddFormattedText(text, font);
            return paragraph;
        }

        public Paragraph Image(String image)
        {
            paragraph.AddImage(image);
            return paragraph;
        }

        public Paragraph AddHorizontalRule(Single width = 0, Color? color = null, Single height = 0)
        {
            if (color.HasValue) paragraph.Format.Borders.Bottom.Color = color.Value;
            if (width != 0) paragraph.Format.Borders.Bottom.Width = width;
            paragraph.Format.Borders.Bottom.Visible = true;
            paragraph.Format.LineSpacing = height;
            paragraph.Format.LineSpacingRule = LineSpacingRule.Exactly;
            return paragraph;
        }
    }

    extension(Image image)
    {
        public Image Width(Single width)
        {
            image.Width = width;
            return image;
        }

        public Image Height(Single height)
        {
            image.Height = height;
            return image;
        }
    }
}