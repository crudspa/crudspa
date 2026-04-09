namespace Crudspa.Education.Provider.Server.Sproxies;

public static class PublisherUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Publisher publisher)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationProvider.PublisherUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", publisher.Id);

        await command.Execute(connection, transaction);
    }
}