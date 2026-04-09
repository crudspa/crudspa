namespace Crudspa.Content.Design.Server.Sproxies;

public static class BlogInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Blog blog)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.BlogInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", blog.PortalId);
        command.AddParameter("@Title", 150, blog.Title);
        command.AddParameter("@StatusId", blog.StatusId);
        command.AddParameter("@Author", 150, blog.Author);
        command.AddParameter("@Description", blog.Description);
        command.AddParameter("@ImageId", blog.ImageFile.Id);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}