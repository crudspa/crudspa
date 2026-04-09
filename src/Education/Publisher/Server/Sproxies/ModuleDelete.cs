namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ModuleDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Module module)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ModuleDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", module.Id);

        await command.Execute(connection, transaction);
    }
}