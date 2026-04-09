namespace Crudspa.Content.Design.Server.Sproxies;

public static class PageDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Page page)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PageDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", page.Id);

        await command.Execute(connection, transaction);
    }
}