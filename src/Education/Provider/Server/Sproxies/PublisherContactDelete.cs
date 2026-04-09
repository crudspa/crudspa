namespace Crudspa.Education.Provider.Server.Sproxies;

public static class PublisherContactDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, PublisherContact publisherContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationProvider.PublisherContactDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", publisherContact.Id);

        await command.Execute(connection, transaction);
    }
}