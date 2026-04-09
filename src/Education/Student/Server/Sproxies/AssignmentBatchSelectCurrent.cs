namespace Crudspa.Education.Student.Server.Sproxies;

public static class AssignmentBatchSelectCurrent
{
    public static async Task<AssignmentBatch?> Execute(String connection, Guid? studentId, Guid? gameId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.AssignmentBatchSelectCurrent";

        command.AddParameter("@StudentId", studentId);
        command.AddParameter("@GameId", gameId);

        return await command.ReadSingle(connection, ReadAssignmentBatch);
    }

    private static AssignmentBatch ReadAssignmentBatch(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Published = reader.ReadDateTimeOffset(1),
        };
    }
}