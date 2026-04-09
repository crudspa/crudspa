namespace Crudspa.Education.Provider.Server.Sproxies;

public static class PublisherInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Publisher publisher)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationProvider.PublisherInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@OrganizationId", publisher.OrganizationId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}