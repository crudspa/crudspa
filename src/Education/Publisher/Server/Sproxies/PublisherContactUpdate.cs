namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class PublisherContactUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, PublisherContact publisherContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.PublisherContactUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", publisherContact.Id);
        command.AddParameter("@UserId", publisherContact.UserId);

        await command.Execute(connection, transaction);
    }
}