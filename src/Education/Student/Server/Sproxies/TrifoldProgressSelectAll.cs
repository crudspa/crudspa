namespace Crudspa.Education.Student.Server.Sproxies;

public static class TrifoldProgressSelectAll
{
    public static async Task<IList<TrifoldProgress>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.TrifoldProgressSelectAll";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadAll(connection, ReadTrifoldProgress);
    }

    public static TrifoldProgress ReadTrifoldProgress(SqlDataReader reader)
    {
        return new()
        {
            StudentId = reader.ReadGuid(0),
            TrifoldId = reader.ReadGuid(1),
            TrifoldCompletedCount = reader.ReadInt32(2),
        };
    }
}