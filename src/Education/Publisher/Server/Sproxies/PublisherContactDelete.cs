namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class PublisherContactDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, PublisherContact publisherContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.PublisherContactDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", publisherContact.Id);

        await command.Execute(connection, transaction);
    }
}