namespace Crudspa.Content.Design.Server.Sproxies;

public static class ForumInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Forum forum)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ForumInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", forum.PortalId);
        command.AddParameter("@Title", 150, forum.Title);
        command.AddParameter("@StatusId", forum.StatusId);
        command.AddParameter("@Description", forum.Description);
        command.AddParameter("@ImageId", forum.ImageFile.Id);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}