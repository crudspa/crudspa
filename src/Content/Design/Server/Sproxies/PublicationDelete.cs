namespace Crudspa.Content.Design.Server.Sproxies;

using Publication = Shared.Contracts.Data.Publication;

public static class PublicationDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Publication publication)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.PublicationDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", publication.Id);

        await command.Execute(connection, transaction);
    }
}