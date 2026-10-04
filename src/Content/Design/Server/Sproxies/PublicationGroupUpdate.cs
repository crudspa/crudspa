
namespace Crudspa.Content.Design.Server.Sproxies;

public static class PublicationGroupUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, PublicationGroup publicationGroup)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PublicationGroupUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", publicationGroup.Id);
        command.AddParameter("@Name", 150, publicationGroup.Name);

        await command.Execute(connection, transaction);
    }
}