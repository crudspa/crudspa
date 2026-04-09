namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class PublisherContactInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, PublisherContact publisherContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.PublisherContactInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ContactId", publisherContact.ContactId);
        command.AddParameter("@UserId", publisherContact.UserId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}