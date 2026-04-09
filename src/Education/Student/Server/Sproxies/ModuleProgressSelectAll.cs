namespace Crudspa.Education.Student.Server.Sproxies;

public static class ModuleProgressSelectAll
{
    public static async Task<IList<ModuleProgress>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ModuleProgressSelectAll";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadAll(connection, ReadModuleProgress);
    }

    public static ModuleProgress ReadModuleProgress(SqlDataReader reader)
    {
        return new()
        {
            StudentId = reader.ReadGuid(0),
            ModuleId = reader.ReadGuid(1),
            BookId = reader.ReadGuid(2),
            ModuleCompletedCount = reader.ReadInt32(3),
        };
    }
}