namespace Crudspa.Framework.Core.Server.Sproxies;

public static class FontFileInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, FontFile fontFile)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.FontFileInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BlobId", fontFile.BlobId);
        command.AddParameter("@Name", 150, fontFile.Name?.Trim());
        command.AddParameter("@Format", 10, fontFile.Name.GetExtension());
        command.AddParameter("@Description", fontFile.Description);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}