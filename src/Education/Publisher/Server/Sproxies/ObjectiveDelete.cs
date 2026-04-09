namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ObjectiveDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Objective objective)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ObjectiveDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", objective.Id);

        await command.Execute(connection, transaction);
    }
}