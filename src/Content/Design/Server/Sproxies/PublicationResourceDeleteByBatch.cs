
namespace Crudspa.Content.Design.Server.Sproxies;

public static class PublicationResourceDeleteByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, PublicationResource publicationResource)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PublicationResourceDeleteByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", publicationResource.Id);

        await command.Execute(connection, transaction);
    }
}