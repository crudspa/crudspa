namespace Crudspa.Framework.Core.Server.Sproxies;

public static class FontFileSelect
{
    public static async Task<FontFile?> Execute(String connection, FontFile fontFile)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.FontFileSelect";

        command.AddParameter("@Id", fontFile.Id);

        return await command.ReadSingle(connection, ReadFontFile);
    }

    private static FontFile ReadFontFile(SqlDataReader reader)
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