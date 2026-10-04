
namespace Crudspa.Content.Design.Server.Sproxies;

public static class PublicationGroupDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, PublicationGroup publicationGroup)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PublicationGroupDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", publicationGroup.Id);

        await command.Execute(connection, transaction);
    }
}