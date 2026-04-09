namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class TrifoldDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Trifold trifold)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.TrifoldDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", trifold.Id);

        await command.Execute(connection, transaction);
    }
}