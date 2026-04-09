namespace Crudspa.Education.Student.Server.Sproxies;

public static class AssignmentBatchInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, AssignmentBatch assignmentBatch)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.AssignmentBatchInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@GameId", assignmentBatch.GameId);
        command.AddParameter("@StudentId", assignmentBatch.StudentId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}