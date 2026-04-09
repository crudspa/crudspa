namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class UnitDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Unit unit)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.UnitDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", unit.Id);

        await command.Execute(connection, transaction);
    }
}