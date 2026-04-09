namespace Crudspa.Education.Student.Server.Sproxies;

public static class ModuleProgressSelect
{
    public static async Task<ModuleProgress> Execute(String connection, Guid? sessionId, Guid? moduleId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ModuleProgressSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ModuleId", moduleId);

        var progress = await command.ReadSingle(connection, ReadModuleProgress);

        return progress ?? new()
        {
            ModuleId = moduleId,
            ModuleCompletedCount = 0,
        };
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