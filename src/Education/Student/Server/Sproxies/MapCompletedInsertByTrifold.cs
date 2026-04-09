namespace Crudspa.Education.Student.Server.Sproxies;

public static class MapCompletedInsertByTrifold
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Guid? trifoldId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.MapCompletedInsertByTrifold";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@TrifoldId", trifoldId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}