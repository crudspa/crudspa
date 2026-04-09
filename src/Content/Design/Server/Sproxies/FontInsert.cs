namespace Crudspa.Content.Design.Server.Sproxies;

public static class FontInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Font font)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.FontInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ContentPortalId", font.ContentPortalId);
        command.AddParameter("@Name", 75, font.Name);
        command.AddParameter("@FileId", font.FileFile.Id);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}