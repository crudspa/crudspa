namespace Crudspa.Framework.Jobs.Server.Sproxies;

public static class JobTypeSelectFull
{
    public static async Task<IList<JobTypeFull>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkJobs.JobTypeSelectFull";

        return await command.ReadAll(connection, ReadJobType);
    }

    private static JobTypeFull ReadJobType(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            EditorView = reader.ReadString(2),
            ActionClass = reader.ReadString(3),
        };
    }
}