namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ListenPartDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ListenPart listenPart)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ListenPartDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", listenPart.Id);

        await command.Execute(connection, transaction);
    }
}