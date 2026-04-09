namespace Crudspa.Framework.Core.Server.Sproxies;

public static class FontFileUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, FontFile fontFile)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.FontFileUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", fontFile.Id);
        command.AddParameter("@BlobId", fontFile.BlobId);
        command.AddParameter("@Name", 150, fontFile.Name?.Trim());
        command.AddParameter("@Format", 10, fontFile.Name.GetExtension());
        command.AddParameter("@Description", fontFile.Description);

        await command.Execute(connection, transaction);
    }
}