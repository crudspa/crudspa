namespace Crudspa.Content.Design.Server.Sproxies;

public static class ForumUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Forum forum)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ForumUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", forum.Id);
        command.AddParameter("@Title", 150, forum.Title);
        command.AddParameter("@StatusId", forum.StatusId);
        command.AddParameter("@Description", forum.Description);
        command.AddParameter("@ImageId", forum.ImageFile.Id);

        await command.Execute(connection, transaction);
    }
}