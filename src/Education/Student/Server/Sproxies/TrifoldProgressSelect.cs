namespace Crudspa.Education.Student.Server.Sproxies;

public static class TrifoldProgressSelect
{
    public static async Task<TrifoldProgress> Execute(String connection, Guid? sessionId, Guid? trifoldId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.TrifoldProgressSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@TrifoldId", trifoldId);

        var progress = await command.ReadSingle(connection, ReadTrifoldProgress);
        return progress ?? new()
        {
            TrifoldId = trifoldId,
            TrifoldCompletedCount = 0,
        };
    }

    public static TrifoldProgress ReadTrifoldProgress(SqlDataReader reader)
    {
        return new()
        {
            StudentId = reader.ReadGuid(0),
            TrifoldId = reader.ReadGuid(1),
            BookId = reader.ReadGuid(2),
            TrifoldCompletedCount = reader.ReadInt32(3),
        };
    }
}