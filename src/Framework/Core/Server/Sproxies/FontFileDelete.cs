namespace Crudspa.Framework.Core.Server.Sproxies;

public static class FontFileDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, FontFile fontFile)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.FontFileDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", fontFile.Id);

        await command.Execute(connection, transaction);
    }
}