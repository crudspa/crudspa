namespace Crudspa.Framework.Core.Server.Sproxies;

public static class PdfFileSelectAndLog
{
    public static async Task<PdfFile?> Execute(String connection, Guid? sessionId, PdfFile pdfFile)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.PdfFileSelectAndLog";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", pdfFile.Id);

        return await command.ReadSingle(connection, ReadPdfFile);
    }

    private static PdfFile ReadPdfFile(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            BlobId = reader.ReadGuid(1),
            Name = reader.ReadString(2),
            Format = reader.ReadString(3),
            Description = reader.ReadString(4),
        };
    }
}